import { useState, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { postService } from '../services/api'
import PostForm from '../components/PostForm'
import Toast from '../components/Toast'

export default function EditPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [post, setPost] = useState(null)
  const [error, setError] = useState(null)
  const [validationErrors, setValidationErrors] = useState({})
  const [loading, setLoading] = useState(false)
  const [toast, setToast] = useState(null)

  useEffect(() => {
    const fetchPost = async () => {
      try {
        const response = await postService.getById(id)
        setPost(response.data)
      } catch (err) {
        setError(err.message)
      }
    }
    fetchPost()
  }, [id])

  const handleSubmit = async (formData) => {
    try {
      setLoading(true)
      setError(null)
      setValidationErrors({})
      await postService.update(id, formData)
      setToast({ message: 'Post updated successfully!', type: 'success' })
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

  if (!post) {
    return (
      <div className="flex flex-col items-center justify-center py-20">
        <div className="w-16 h-16 border-4 border-primary-200 border-t-primary-600 rounded-full animate-spin"></div>
        <p className="mt-4 text-gray-600 font-medium">Loading post data...</p>
      </div>
    )
  }

  return (
    <div className="max-w-3xl mx-auto">
      {/* Header */}
      <div className="mb-8 text-center">
        <div className="inline-flex items-center justify-center w-16 h-16 bg-gradient-to-br from-accent-500 to-primary-500 rounded-2xl mb-4">
          <span className="text-3xl">✏️</span>
        </div>
        <h1 className="text-4xl font-bold text-gray-900 mb-2">
          Edit <span className="text-gradient">Post</span>
        </h1>
        <p className="text-gray-600">Update your content and make it even better</p>
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
        initialData={post} 
        onSubmit={handleSubmit} 
        loading={loading}
        isEdit={true}
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
