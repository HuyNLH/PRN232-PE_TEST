import { useState, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { Edit, ArrowLeft } from 'lucide-react'
import { moviesApi } from '../api'
import MovieForm from '../components/MovieForm'
import Toast from '../components/Toast'

function EditMovie() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [movie, setMovie] = useState(null)
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)
  const [toast, setToast] = useState(null)
  const [error, setError] = useState(null)

  useEffect(() => {
    fetchMovie()
  }, [id])

  const fetchMovie = async () => {
    try {
      setLoading(true)
      setError(null)
      const response = await moviesApi.getById(id)
      setMovie(response.data)
    } catch (err) {
      console.error('Error fetching movie:', err)
      setError('Movie not found')
      showToast('Failed to load movie', 'error')
    } finally {
      setLoading(false)
    }
  }

  const handleSubmit = async (movieData) => {
    try {
      setSubmitting(true)
      
      // Create FormData for file upload support
      const formData = new FormData()
      formData.append('title', movieData.title)
      if (movieData.genre) formData.append('genre', movieData.genre)
      if (movieData.rating) formData.append('rating', movieData.rating)
      
      // Handle poster image (file or URL)
      if (movieData.posterFile) {
        formData.append('posterFile', movieData.posterFile)
      } else if (movieData.posterImage) {
        formData.append('posterImage', movieData.posterImage)
      }
      
      await moviesApi.update(id, formData)
      
      showToast('Movie updated successfully!', 'success')
      setTimeout(() => navigate('/'), 1500)
    } catch (err) {
      console.error('Error updating movie:', err)
      
      // Handle validation errors
      if (err.response?.data?.errors) {
        const serverErrors = err.response.data.errors
        const errorMessages = Object.values(serverErrors).flat().join(', ')
        showToast(errorMessages || 'Validation failed', 'error')
      } else {
        showToast(err.response?.data?.message || 'Failed to update movie', 'error')
      }
      setSubmitting(false)
      throw err
    }
  }

  const handleCancel = () => {
    navigate('/')
  }

  const showToast = (message, type = 'success') => {
    setToast({ message, type })
    setTimeout(() => setToast(null), 3000)
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[60vh]">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-500 mx-auto"></div>
          <p className="mt-4 text-gray-600">Loading movie...</p>
        </div>
      </div>
    )
  }

  if (error || !movie) {
    return (
      <div className="max-w-3xl mx-auto">
        <div className="bg-red-50 border border-red-200 text-red-700 px-6 py-4 rounded-lg">
          <h3 className="font-semibold">Error</h3>
          <p>{error || 'Movie not found'}</p>
          <button
            onClick={() => navigate('/')}
            className="mt-4 text-sm underline hover:no-underline"
          >
            Go back to movies
          </button>
        </div>
      </div>
    )
  }

  return (
    <div className="max-w-3xl mx-auto">
      {/* Header */}
      <div className="mb-8">
        <button
          onClick={handleCancel}
          className="inline-flex items-center gap-2 text-gray-600 hover:text-primary-600 mb-4 transition-colors"
        >
          <ArrowLeft size={20} />
          <span>Back to Movies</span>
        </button>
        
        <div className="flex items-center gap-3">
          <div className="w-12 h-12 bg-gradient-to-br from-primary-500 to-accent-500 rounded-xl flex items-center justify-center">
            <Edit className="text-white" size={24} />
          </div>
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Edit Movie</h1>
            <p className="text-gray-500">Update the movie details</p>
          </div>
        </div>
      </div>

      {/* Form Card */}
      <div className="bg-white rounded-2xl shadow-lg border border-gray-200/50 p-8">
        <MovieForm
          initialData={movie}
          onSubmit={handleSubmit}
          isSubmitting={submitting}
          mode="edit"
        />
      </div>

      {/* Toast Notification */}
      {toast && <Toast message={toast.message} type={toast.type} onClose={() => setToast(null)} />}
    </div>
  )
}

export default EditMovie
