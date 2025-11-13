import { useState, useEffect, useCallback } from 'react'
import { Film, Search, Filter } from 'lucide-react'
import { moviesApi } from '../api'
import MovieCard from '../components/MovieCard'
import SearchFilterBar from '../components/SearchFilterBar'
import Toast from '../components/Toast'

function MovieList() {
  const [movies, setMovies] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [searchQuery, setSearchQuery] = useState('')
  const [debouncedSearch, setDebouncedSearch] = useState('')
  const [genreFilter, setGenreFilter] = useState('')
  const [sortBy, setSortBy] = useState('rating_desc')
  const [toast, setToast] = useState(null)
  const [availableGenres, setAvailableGenres] = useState([])

  // Debounce search query
  useEffect(() => {
    const timeoutId = setTimeout(() => {
      setDebouncedSearch(searchQuery)
    }, 500)
    return () => clearTimeout(timeoutId)
  }, [searchQuery])

  useEffect(() => {
    fetchMovies()
  }, [debouncedSearch, genreFilter, sortBy])

  const fetchMovies = async () => {
    try {
      setLoading(true)
      setError(null)
      
      const params = {}
      if (debouncedSearch.trim()) params.q = debouncedSearch.trim()
      if (genreFilter) params.genre = genreFilter
      if (sortBy) params.sort = sortBy
      
      const response = await moviesApi.getAll(params)
      setMovies(response.data)
      
      // Extract unique genres from all movies for filter dropdown
      if (!genreFilter && !debouncedSearch) {
        const genres = [...new Set(response.data.map(m => m.genre).filter(Boolean))]
        setAvailableGenres(genres.sort())
      }
    } catch (err) {
      console.error('Error fetching movies:', err)
      setError('Failed to load movies. Please try again.')
      showToast('Failed to load movies', 'error')
    } finally {
      setLoading(false)
    }
  }

  const handleDelete = async (id) => {
    try {
      await moviesApi.delete(id)
      setMovies(movies.filter(movie => movie.id !== id))
      showToast('Movie deleted successfully!', 'success')
    } catch (err) {
      console.error('Error deleting movie:', err)
      showToast('Failed to delete movie', 'error')
    }
  }

  const showToast = (message, type = 'success') => {
    setToast({ message, type })
    setTimeout(() => setToast(null), 3000)
  }

  if (loading && movies.length === 0) {
    return (
      <div className="flex items-center justify-center min-h-[60vh]">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-500 mx-auto"></div>
          <p className="mt-4 text-gray-600">Loading movies...</p>
        </div>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div className="w-12 h-12 bg-gradient-to-br from-primary-500 to-accent-500 rounded-xl flex items-center justify-center">
            <Film className="text-white" size={24} />
          </div>
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Movie Watchlist</h1>
            <p className="text-gray-500">{movies.length} movies in your collection</p>
          </div>
        </div>
      </div>

      {/* Search and Filter Bar */}
      <SearchFilterBar
        searchQuery={searchQuery}
        onSearchChange={setSearchQuery}
        genreFilter={genreFilter}
        onGenreChange={setGenreFilter}
        sortBy={sortBy}
        onSortChange={setSortBy}
        onRefresh={fetchMovies}
        isLoading={loading}
        availableGenres={availableGenres}
      />

      {/* Error Message */}
      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg">
          {error}
        </div>
      )}

      {/* Movies Grid */}
      {movies.length === 0 && !loading ? (
        <div className="text-center py-12">
          <Film className="mx-auto h-16 w-16 text-gray-400" />
          <h3 className="mt-4 text-lg font-medium text-gray-900">No movies found</h3>
          <p className="mt-2 text-gray-500">
            {searchQuery || genreFilter 
              ? 'Try adjusting your search or filters' 
              : 'Get started by adding your first movie!'}
          </p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          {movies.map(movie => (
            <MovieCard
              key={movie.id}
              movie={movie}
              onDelete={handleDelete}
            />
          ))}
        </div>
      )}

      {/* Toast Notification */}
      {toast && <Toast message={toast.message} type={toast.type} onClose={() => setToast(null)} />}
    </div>
  )
}

export default MovieList
