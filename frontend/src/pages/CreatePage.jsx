import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { postService } from '../services/api'
import PostForm from '../components/PostForm'
import Toast from '../components/Toast'

export default function CreatePage() {
  const navigate = useNavigate()
  const [error, setError] = useState(null)
  const [validationErrors, setValidationErrors] = useState({})
  const [loading, setLoading] = useState(false)
  const [toast, setToast] = useState(null)

  const handleSubmit = async (formData) => {
    try {
      setLoading(true)
      setError(null)
      setValidationErrors({})
      await postService.create(formData)
      setToast({ message: 'Post created successfully!', type: 'success' })
      setTimeout(() => {
        navigate('/')
      }, 1000)
    } catch (err) {
      if (err.response?.data?.errors) {
        // Backend validation errors
        setValidationErrors(err.response.data.errors)
        setError('Please fix the validation errors below')
      } else {
        setError(err.response?.data?.message || err.message)
      }
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="max-w-3xl mx-auto">
      {/* Header */}
      <div className="mb-8 text-center">
        <div className="inline-flex items-center justify-center w-16 h-16 bg-gradient-to-br from-primary-500 to-accent-500 rounded-2xl mb-4">
          <span className="text-3xl">✨</span>
        </div>
        <h1 className="text-4xl font-bold text-gray-900 mb-2">
          Create New <span className="text-gradient">Post</span>
        </h1>
        <p className="text-gray-600">Share your amazing content with the world</p>
      </div>
      
      {error && (
        <div className="mb-6 bg-gradient-to-r from-danger-50 to-danger-100 border-l-4 border-danger-500 text-danger-800 px-6 py-4 rounded-xl shadow-soft animate-fade-in">
          <div className="flex items-center gap-3">
            <span className="text-2xl">⚠️</span>
            <div>
              <h3 className="font-bold">Validation Error</h3>
              <p className="text-sm">{error}</p>
            </div>
          </div>
        </div>
      )}

      <PostForm 
        onSubmit={handleSubmit} 
        loading={loading}
        validationErrors={validationErrors}
      />

      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}
    </div>
  )
}
