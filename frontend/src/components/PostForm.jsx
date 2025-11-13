import { useState, useEffect } from 'react'

export default function PostForm({ initialData = {}, onSubmit, loading, isEdit = false, validationErrors = {} }) {
  const [formData, setFormData] = useState({
    name: initialData.name || '',
    description: initialData.description || '',
    imageUrl: initialData.image || '',
  })
  const [imageFile, setImageFile] = useState(null)
  const [imagePreview, setImagePreview] = useState(initialData.image || null)

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData(prev => ({ ...prev, [name]: value }))
    
    // Update image preview when URL changes
    if (name === 'imageUrl' && value && !imageFile) {
      setImagePreview(value)
    }
  }

  const handleFileChange = (e) => {
    const file = e.target.files[0]
    if (file) {
      // Validate file size (5MB)
      if (file.size > 5 * 1024 * 1024) {
        alert('File size must be less than 5MB')
        e.target.value = null
        return
      }
      
      // Validate file type
      const validTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp']
      if (!validTypes.includes(file.type)) {
        alert('Only JPEG, PNG, GIF, and WebP images are allowed')
        e.target.value = null
        return
      }

      setImageFile(file)
      setFormData(prev => ({ ...prev, imageUrl: '' })) // Clear URL when file selected
      
      const reader = new FileReader()
      reader.onloadend = () => {
        setImagePreview(reader.result)
      }
      reader.readAsDataURL(file)
    }
  }

  // Update preview when imageUrl changes
  useEffect(() => {
    if (formData.imageUrl && !imageFile) {
      setImagePreview(formData.imageUrl)
    }
  }, [formData.imageUrl, imageFile])

  const handleSubmit = (e) => {
    e.preventDefault()
    
    const submitData = new FormData()
    submitData.append('Name', formData.name)
    submitData.append('Description', formData.description)
    
    if (imageFile) {
      submitData.append('ImageFile', imageFile)
    } else if (formData.imageUrl) {
      submitData.append('ImageUrl', formData.imageUrl)
    }

    onSubmit(submitData)
  }

  return (
    <form onSubmit={handleSubmit} className="bg-white/80 backdrop-blur-lg shadow-soft rounded-3xl p-8 border border-gray-100">
      {/* Name Field */}
      <div className="mb-6">
        <label className="block text-gray-800 text-sm font-bold mb-2 flex items-center gap-2" htmlFor="name">
          <span className="text-lg">✍️</span>
          Post Name
          <span className="text-xs font-normal text-gray-500">(3-200 characters)</span>
        </label>
        <input
          id="name"
          name="name"
          type="text"
          value={formData.name}
          onChange={handleChange}
          required
          minLength={3}
          maxLength={200}
          placeholder="Enter an amazing post name..."
          className={`w-full px-4 py-3 border-2 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary-500 transition-all ${
            validationErrors.Name ? 'border-danger-500 bg-danger-50' : 'border-gray-200 bg-white'
          }`}
        />
        {validationErrors.Name && (
          <div className="mt-2 flex items-center gap-2 text-danger-600 text-sm">
            <span>⚠️</span>
            <p>{validationErrors.Name[0]}</p>
          </div>
        )}
      </div>

      {/* Description Field */}
      <div className="mb-6">
        <label className="block text-gray-800 text-sm font-bold mb-2 flex items-center gap-2" htmlFor="description">
          <span className="text-lg">📝</span>
          Description
          <span className="text-xs font-normal text-gray-500">(10-2000 characters)</span>
        </label>
        <textarea
          id="description"
          name="description"
          value={formData.description}
          onChange={handleChange}
          required
          minLength={10}
          maxLength={2000}
          rows={5}
          placeholder="Tell us more about your post..."
          className={`w-full px-4 py-3 border-2 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary-500 transition-all resize-none ${
            validationErrors.Description ? 'border-danger-500 bg-danger-50' : 'border-gray-200 bg-white'
          }`}
        />
        {validationErrors.Description && (
          <div className="mt-2 flex items-center gap-2 text-danger-600 text-sm">
            <span>⚠️</span>
            <p>{validationErrors.Description[0]}</p>
          </div>
        )}
        <div className="mt-2 flex items-center justify-between">
          <p className="text-xs text-gray-500">
            💡 Tip: Be descriptive and engaging!
          </p>
          <p className={`text-sm font-medium ${
            formData.description.length > 1900 ? 'text-warning-600' : 'text-gray-600'
          }`}>
            {formData.description.length} / 2000
          </p>
        </div>
      </div>

      {/* Image Upload Section */}
      <div className="mb-6 p-6 bg-gradient-to-br from-primary-50 to-accent-50 rounded-2xl border-2 border-dashed border-primary-200">
        <h3 className="font-bold text-gray-800 mb-4 flex items-center gap-2">
          <span className="text-xl">🖼️</span>
          Add Image
        </h3>

        {/* File Upload */}
        <div className="mb-4">
          <label className="block text-gray-700 text-sm font-semibold mb-2" htmlFor="imageFile">
            Upload from Device
            <span className="text-xs font-normal text-gray-500 ml-2">(Max 5MB - JPEG, PNG, GIF, WebP)</span>
          </label>
          <input
            id="imageFile"
            type="file"
            accept="image/jpeg,image/png,image/gif,image/webp"
            onChange={handleFileChange}
            className="w-full px-4 py-3 border-2 border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary-500 bg-white file:mr-4 file:py-2 file:px-4 file:rounded-lg file:border-0 file:bg-primary-500 file:text-white file:font-medium hover:file:bg-primary-600 transition-all"
          />
          {imageFile && (
            <div className="mt-2 flex items-center gap-2 text-success-600 text-sm font-medium">
              <span>✅</span>
              <p>File selected: {imageFile.name}</p>
            </div>
          )}
        </div>

        {/* URL Input */}
        <div className="mb-4">
          <label className="block text-gray-700 text-sm font-semibold mb-2" htmlFor="imageUrl">
            Or Enter Image URL
            <span className="text-xs font-normal text-gray-500 ml-2">(Max 500 characters)</span>
          </label>
          <input
            id="imageUrl"
            name="imageUrl"
            type="url"
            value={formData.imageUrl}
            onChange={handleChange}
            maxLength={500}
            placeholder="https://example.com/image.jpg"
            disabled={imageFile !== null}
            className={`w-full px-4 py-3 border-2 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary-500 transition-all ${
              imageFile ? 'bg-gray-100 cursor-not-allowed border-gray-200' : 'border-gray-200 bg-white'
            }`}
          />
          {imageFile && (
            <p className="text-gray-500 text-xs mt-2">
              💡 Clear file selection to use URL instead
            </p>
          )}
        </div>

        {/* Image Preview */}
        {imagePreview && (
          <div className="mt-4">
            <label className="block text-gray-700 text-sm font-semibold mb-3">Preview</label>
            <div className="relative bg-white border-2 border-gray-200 rounded-2xl p-3 overflow-hidden">
              <img 
                src={imagePreview} 
                alt="Preview" 
                className="max-w-full max-h-80 object-contain mx-auto rounded-xl"
                onError={(e) => {
                  e.target.style.display = 'none'
                  e.target.nextSibling.style.display = 'block'
                }}
              />
              <div className="text-danger-500 text-sm text-center mt-3 p-3 bg-danger-50 rounded-lg" style={{ display: 'none' }}>
                ⚠️ Failed to load image preview
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Action Buttons */}
      <div className="flex gap-4 pt-6 border-t border-gray-200">
        <button
          type="submit"
          disabled={loading}
          className="flex-1 bg-gradient-to-r from-primary-500 to-primary-600 text-white font-bold py-4 px-6 rounded-xl hover:from-primary-600 hover:to-primary-700 disabled:opacity-50 disabled:cursor-not-allowed transform hover:scale-105 transition-all duration-200 shadow-lg shadow-primary-500/30 flex items-center justify-center gap-2"
        >
          {loading ? (
            <>
              <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
              <span>Saving...</span>
            </>
          ) : (
            <>
              <span>{isEdit ? '💾' : '✨'}</span>
              <span>{isEdit ? 'Update Post' : 'Create Post'}</span>
            </>
          )}
        </button>
        <button
          type="button"
          onClick={() => window.history.back()}
          className="flex-1 bg-gray-100 text-gray-700 font-bold py-4 px-6 rounded-xl hover:bg-gray-200 transition-all duration-200 flex items-center justify-center gap-2"
        >
          <span>❌</span>
          <span>Cancel</span>
        </button>
      </div>
    </form>
  )
}
