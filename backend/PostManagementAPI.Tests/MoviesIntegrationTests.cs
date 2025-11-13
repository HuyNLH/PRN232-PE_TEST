using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using PostManagementAPI.DTOs;

namespace PostManagementAPI.Tests
{
    /// <summary>
    /// Integration tests for Movies API endpoints
    /// Tests: Create valid movie, Create invalid movie, Get filtered list
    /// </summary>
    public class MoviesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public MoviesIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateMovie_WithValidData_ReturnsCreated()
        {
            // Arrange
            var newMovie = new CreateMovieDto
            {
                Title = "Test Movie",
                Genre = "Action",
                Rating = 4,
                PosterImage = "https://example.com/poster.jpg"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/movies", newMovie);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var createdMovie = await response.Content.ReadFromJsonAsync<MovieResponseDto>();
            createdMovie.Should().NotBeNull();
            createdMovie!.Title.Should().Be(newMovie.Title);
            createdMovie.Genre.Should().Be(newMovie.Genre);
            createdMovie.Rating.Should().Be(newMovie.Rating);
            createdMovie.PosterImage.Should().Be(newMovie.PosterImage);
            createdMovie.Id.Should().BeGreaterThan(0);
            
            // Verify location header
            response.Headers.Location.Should().NotBeNull();
            response.Headers.Location!.ToString().Should().Contain($"/api/movies/{createdMovie.Id}");
        }

        [Fact]
        public async Task CreateMovie_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange - Missing required Title field
            var invalidMovie = new
            {
                Title = "", // Empty title (violates [Required] and [StringLength(min=1)])
                Genre = "Action",
                Rating = 4
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/movies", invalidMovie);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CreateMovie_WithInvalidRating_ReturnsBadRequest()
        {
            // Arrange - Rating out of range (must be 1-5)
            var invalidMovie = new CreateMovieDto
            {
                Title = "Invalid Rating Movie",
                Genre = "Drama",
                Rating = 10 // Invalid: exceeds max of 5
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/movies", invalidMovie);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateMovie_WithTooLongTitle_ReturnsBadRequest()
        {
            // Arrange - Title exceeds 200 characters
            var invalidMovie = new CreateMovieDto
            {
                Title = new string('A', 201), // Exceeds max length
                Genre = "Drama",
                Rating = 3
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/movies", invalidMovie);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetMovies_WithGenreFilter_ReturnsFilteredList()
        {
            // Arrange - Create test movies
            var actionMovie = new CreateMovieDto
            {
                Title = "Action Movie 1",
                Genre = "Action",
                Rating = 5
            };
            var dramaMovie = new CreateMovieDto
            {
                Title = "Drama Movie 1",
                Genre = "Drama",
                Rating = 4
            };

            await _client.PostAsJsonAsync("/api/movies", actionMovie);
            await _client.PostAsJsonAsync("/api/movies", dramaMovie);

            // Act - Filter by Action genre
            var response = await _client.GetAsync("/api/movies?genre=Action");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var movies = await response.Content.ReadFromJsonAsync<List<MovieResponseDto>>();
            movies.Should().NotBeNull();
            movies!.Should().NotBeEmpty();
            movies.Should().OnlyContain(m => m.Genre == "Action");
        }

        [Fact]
        public async Task GetMovies_WithSearchQuery_ReturnsMatchingMovies()
        {
            // Arrange - Create test movies
            var movie1 = new CreateMovieDto
            {
                Title = "The Matrix",
                Genre = "Sci-Fi",
                Rating = 5
            };
            var movie2 = new CreateMovieDto
            {
                Title = "The Godfather",
                Genre = "Crime",
                Rating = 5
            };

            await _client.PostAsJsonAsync("/api/movies", movie1);
            await _client.PostAsJsonAsync("/api/movies", movie2);

            // Act - Search for "Matrix"
            var response = await _client.GetAsync("/api/movies?q=Matrix");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var movies = await response.Content.ReadFromJsonAsync<List<MovieResponseDto>>();
            movies.Should().NotBeNull();
            movies!.Should().ContainSingle();
            movies![0].Title.Should().Contain("Matrix");
        }

        [Fact]
        public async Task GetMovies_WithRatingSortDescending_ReturnsSortedList()
        {
            // Arrange - Create movies with different ratings
            var movies = new[]
            {
                new CreateMovieDto { Title = "Low Rating", Genre = "Action", Rating = 2 },
                new CreateMovieDto { Title = "High Rating", Genre = "Drama", Rating = 5 },
                new CreateMovieDto { Title = "Medium Rating", Genre = "Comedy", Rating = 3 }
            };

            foreach (var movie in movies)
            {
                await _client.PostAsJsonAsync("/api/movies", movie);
            }

            // Act - Sort by rating descending
            var response = await _client.GetAsync("/api/movies?sort=rating_desc");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var sortedMovies = await response.Content.ReadFromJsonAsync<List<MovieResponseDto>>();
            sortedMovies.Should().NotBeNull();
            sortedMovies!.Count.Should().BeGreaterThanOrEqualTo(3);
            
            // Verify descending order
            for (int i = 0; i < sortedMovies.Count - 1; i++)
            {
                var currentRating = sortedMovies[i].Rating ?? 0;
                var nextRating = sortedMovies[i + 1].Rating ?? 0;
                currentRating.Should().BeGreaterThanOrEqualTo(nextRating);
            }
        }

        [Fact]
        public async Task GetMovieById_WhenExists_ReturnsMovie()
        {
            // Arrange - Create a movie
            var newMovie = new CreateMovieDto
            {
                Title = "Test Movie for Get",
                Genre = "Thriller",
                Rating = 4
            };
            
            var createResponse = await _client.PostAsJsonAsync("/api/movies", newMovie);
            var createdMovie = await createResponse.Content.ReadFromJsonAsync<MovieResponseDto>();

            // Act - Get the movie by ID
            var response = await _client.GetAsync($"/api/movies/{createdMovie!.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var movie = await response.Content.ReadFromJsonAsync<MovieResponseDto>();
            movie.Should().NotBeNull();
            movie!.Id.Should().Be(createdMovie.Id);
            movie.Title.Should().Be(newMovie.Title);
        }

        [Fact]
        public async Task GetMovieById_WhenNotExists_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/movies/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateMovie_WithValidData_ReturnsOk()
        {
            // Arrange - Create a movie first
            var newMovie = new CreateMovieDto
            {
                Title = "Original Title",
                Genre = "Action",
                Rating = 3
            };
            
            var createResponse = await _client.PostAsJsonAsync("/api/movies", newMovie);
            var createdMovie = await createResponse.Content.ReadFromJsonAsync<MovieResponseDto>();

            var updateMovie = new UpdateMovieDto
            {
                Title = "Updated Title",
                Genre = "Drama",
                Rating = 5
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/movies/{createdMovie!.Id}", updateMovie);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var updatedMovie = await response.Content.ReadFromJsonAsync<MovieResponseDto>();
            updatedMovie.Should().NotBeNull();
            updatedMovie!.Title.Should().Be(updateMovie.Title);
            updatedMovie.Genre.Should().Be(updateMovie.Genre);
            updatedMovie.Rating.Should().Be(updateMovie.Rating);
        }

        [Fact]
        public async Task DeleteMovie_WhenExists_ReturnsNoContent()
        {
            // Arrange - Create a movie first
            var newMovie = new CreateMovieDto
            {
                Title = "Movie to Delete",
                Genre = "Horror",
                Rating = 3
            };
            
            var createResponse = await _client.PostAsJsonAsync("/api/movies", newMovie);
            var createdMovie = await createResponse.Content.ReadFromJsonAsync<MovieResponseDto>();

            // Act
            var response = await _client.DeleteAsync($"/api/movies/{createdMovie!.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            
            // Verify it's actually deleted
            var getResponse = await _client.GetAsync($"/api/movies/{createdMovie.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetMovies_WithPagination_ReturnsPagedResults()
        {
            // Arrange - Create multiple movies
            for (int i = 1; i <= 10; i++)
            {
                var movie = new CreateMovieDto
                {
                    Title = $"Movie {i}",
                    Genre = "Action",
                    Rating = 4
                };
                await _client.PostAsJsonAsync("/api/movies", movie);
            }

            // Act - Get first page with 5 items
            var response = await _client.GetAsync("/api/movies?page=1&pageSize=5");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var movies = await response.Content.ReadFromJsonAsync<List<MovieResponseDto>>();
            movies.Should().NotBeNull();
            movies!.Count.Should().BeLessThanOrEqualTo(5);
        }
    }
}
