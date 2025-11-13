import { useState, useEffect } from 'react'
import { Upload, X, Star } from 'lucide-react'

function MovieForm({ initialData = null, onSubmit, isSubmitting = false, mode = 'create' }) {
  const [formData, setFormData] = useState({
    title: '',
    genre: '',
    rating: 0,
    posterImage: ''
  })
  const [posterFile, setPosterFile] = useState(null)
  const [posterPreview, setPosterPreview] = useState(null)
  const [errors, setErrors] = useState({})
  const [isDragging, setIsDragging] = useState(false)

  useEffect(() => {
    if (initialData) {
      setFormData({
        title: initialData.title || '',
        genre: initialData.genre || '',
        rating: initialData.rating || 0,
        posterImage: initialData.posterImage || ''
      })
      if (initialData.posterImage) {
        setPosterPreview(initialData.posterImage)
      }
    }
  }, [initialData])

  const genres = [
    'Action',
    'Drama',
    'Crime',
    'Sci-Fi',
    'Thriller',
    'Comedy',
    'Romance',
    'Horror',
    'Fantasy',
    'Adventure',
    'Animation'
  ]

  const validateForm = () => {
    const newErrors = {}
    
    if (!formData.title.trim()) {
      newErrors.title = 'Title is required'
    }
    
    if (formData.rating && (formData.rating < 1 || formData.rating > 5)) {
      newErrors.rating = 'Rating must be between 1 and 5'
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleInputChange = (e) => {
    const { name, value } = e.target
    setFormData(prev => ({
      ...prev,
      [name]: value
    }))
    // Clear error for this field
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }))
    }
  }

  const handleRatingChange = (newRating) => {
    setFormData(prev => ({
      ...prev,
      rating: newRating
    }))
    if (errors.rating) {
      setErrors(prev => ({ ...prev, rating: '' }))
    }
  }

  const handleFileSelect = (file) => {
    if (file && file.type.startsWith('image/')) {
      setPosterFile(file)
      const reader = new FileReader()
      reader.onloadend = () => {
        setPosterPreview(reader.result)
      }
      reader.readAsDataURL(file)
      // Clear posterImage URL if file is selected
      setFormData(prev => ({ ...prev, posterImage: '' }))
    }
  }

  const handleFileChange = (e) => {
    const file = e.target.files?.[0]
    if (file) {
      handleFileSelect(file)
    }
  }

  const handleDragOver = (e) => {
    e.preventDefault()
    setIsDragging(true)
  }

  const handleDragLeave = (e) => {
    e.preventDefault()
    setIsDragging(false)
  }

  const handleDrop = (e) => {
    e.preventDefault()
    setIsDragging(false)
    const file = e.dataTransfer.files?.[0]
    if (file) {
      handleFileSelect(file)
    }
  }

  const handleRemovePoster = () => {
    setPosterFile(null)
    setPosterPreview(null)
    setFormData(prev => ({ ...prev, posterImage: '' }))
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    
    if (!validateForm()) {
      return
    }

    await onSubmit({
      ...formData,
      posterFile
    })
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      {/* Title */}
      <div>
        <label htmlFor="title" className="block text-sm font-medium text-gray-700 mb-2">
          Title <span className="text-red-500">*</span>
        </label>
        <input
          type="text"
          id="title"
          name="title"
          value={formData.title}
          onChange={handleInputChange}
          className={`w-full px-4 py-2.5 border ${errors.title ? 'border-red-500' : 'border-gray-300'} rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent transition-all`}
          placeholder="Enter movie title"
          disabled={isSubmitting}
        />
        {errors.title && (
          <p className="mt-1 text-sm text-red-500">{errors.title}</p>
        )}
      </div>

      {/* Genre */}
      <div>
        <label htmlFor="genre" className="block text-sm font-medium text-gray-700 mb-2">
          Genre
        </label>
        <select
          id="genre"
          name="genre"
          value={formData.genre}
          onChange={handleInputChange}
          className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent appearance-none bg-white transition-all cursor-pointer"
          disabled={isSubmitting}
        >
          <option value="">Select a genre</option>
          {genres.map((genre) => (
            <option key={genre} value={genre}>
              {genre}
            </option>
          ))}
        </select>
      </div>

      {/* Rating */}
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Rating
        </label>
        <div className="flex items-center gap-2">
          {[1, 2, 3, 4, 5].map((star) => (
            <button
              key={star}
              type="button"
              onClick={() => handleRatingChange(star)}
              disabled={isSubmitting}
              className="transition-transform hover:scale-110 disabled:cursor-not-allowed"
            >
              <Star
                size={32}
                className={star <= formData.rating ? 'fill-yellow-400 text-yellow-400' : 'text-gray-300'}
              />
            </button>
          ))}
          {formData.rating > 0 && (
            <button
              type="button"
              onClick={() => handleRatingChange(0)}
              disabled={isSubmitting}
              className="ml-2 text-sm text-gray-500 hover:text-gray-700 disabled:cursor-not-allowed"
            >
              Clear
            </button>
          )}
        </div>
        {errors.rating && (
          <p className="mt-1 text-sm text-red-500">{errors.rating}</p>
        )}
      </div>

      {/* Poster Upload */}
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Poster Image
        </label>
        
        {!posterPreview ? (
          <div
            onDragOver={handleDragOver}
            onDragLeave={handleDragLeave}
            onDrop={handleDrop}
            className={`border-2 border-dashed ${isDragging ? 'border-primary-500 bg-primary-50' : 'border-gray-300'} rounded-lg p-8 text-center transition-all`}
          >
            <Upload className="mx-auto text-gray-400 mb-4" size={48} />
            <p className="text-gray-600 mb-2">Drag and drop an image here, or</p>
            <label className="inline-block px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors cursor-pointer">
              <span>Choose File</span>
              <input
                type="file"
                accept="image/*"
                onChange={handleFileChange}
                className="hidden"
                disabled={isSubmitting}
              />
            </label>
          </div>
        ) : (
          <div className="relative inline-block">
            <img
              src={posterPreview}
              alt="Poster preview"
              className="w-48 h-72 object-cover rounded-lg border-2 border-gray-200"
            />
            <button
              type="button"
              onClick={handleRemovePoster}
              disabled={isSubmitting}
              className="absolute -top-2 -right-2 p-1.5 bg-red-500 text-white rounded-full hover:bg-red-600 transition-colors shadow-lg disabled:cursor-not-allowed"
            >
              <X size={18} />
            </button>
          </div>
        )}
        
        {/* Or provide URL */}
        <div className="mt-4">
          <label htmlFor="posterImage" className="block text-sm font-medium text-gray-700 mb-2">
            Or provide image URL
          </label>
          <input
            type="url"
            id="posterImage"
            name="posterImage"
            value={formData.posterImage}
            onChange={handleInputChange}
            className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent transition-all"
            placeholder="https://example.com/poster.jpg"
            disabled={isSubmitting || posterFile !== null}
          />
        </div>
      </div>

      {/* Submit Button */}
      <div className="flex justify-end gap-3 pt-4">
        <button
          type="button"
          onClick={() => window.history.back()}
          disabled={isSubmitting}
          className="px-6 py-2.5 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors font-medium disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Cancel
        </button>
        <button
          type="submit"
          disabled={isSubmitting}
          className="px-6 py-2.5 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors font-medium disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2"
        >
          {isSubmitting ? (
            <>
              <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
              <span>{mode === 'create' ? 'Creating...' : 'Updating...'}</span>
            </>
          ) : (
            <span>{mode === 'create' ? 'Create Movie' : 'Update Movie'}</span>
          )}
        </button>
      </div>
    </form>
  )
}

export default MovieForm
