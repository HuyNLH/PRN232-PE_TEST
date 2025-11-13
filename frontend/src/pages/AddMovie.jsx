import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { PlusCircle, ArrowLeft } from 'lucide-react'
import { moviesApi } from '../api'
import MovieForm from '../components/MovieForm'
import Toast from '../components/Toast'

function AddMovie() {
  const navigate = useNavigate()
  const [loading, setLoading] = useState(false)
  const [toast, setToast] = useState(null)

  const handleSubmit = async (movieData) => {
    try {
      setLoading(true)
      
      // Create FormData for file upload support
      const formData = new FormData()
      formData.append('title', movieData.title)
      if (movieData.genre) formData.append('genre', movieData.genre)
      if (movieData.rating) formData.append('rating', movieData.rating)
      
      // Handle poster image (file or URL)
      if (movieData.posterFile) {
        formData.append('PosterFile', movieData.posterFile)
      } else if (movieData.posterImage) {
        formData.append('PosterUrl', movieData.posterImage)
      }
      
      await moviesApi.create(formData)
      
      showToast('Movie added successfully!', 'success')
      setTimeout(() => navigate('/'), 1500)
    } catch (err) {
      console.error('Error creating movie:', err)
      
      // Handle validation errors
      if (err.response?.data?.errors) {
        const serverErrors = err.response.data.errors
        const errorMessages = Object.values(serverErrors).flat().join(', ')
        showToast(errorMessages || 'Validation failed', 'error')
      } else {
        showToast(err.response?.data?.message || 'Failed to add movie', 'error')
      }
      setLoading(false)
      throw err // Re-throw to let form handle it
    }
  }

  const handleCancel = () => {
    navigate('/')
  }

  const showToast = (message, type = 'success') => {
    setToast({ message, type })
    setTimeout(() => setToast(null), 3000)
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
            <PlusCircle className="text-white" size={24} />
          </div>
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Add New Movie</h1>
            <p className="text-gray-500">Fill in the details to add a movie to your watchlist</p>
          </div>
        </div>
      </div>

      {/* Form Card */}
      <div className="bg-white rounded-2xl shadow-lg border border-gray-200/50 p-8">
        <MovieForm
          onSubmit={handleSubmit}
          isSubmitting={loading}
          mode="create"
        />
      </div>

      {/* Toast Notification */}
      {toast && <Toast message={toast.message} type={toast.type} onClose={() => setToast(null)} />}
    </div>
  )
}

export default AddMovie
