import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Film, Edit, Trash2, Star, Calendar } from 'lucide-react'
import ConfirmModal from './ConfirmModal'

function MovieCard({ movie, onDelete }) {
  const navigate = useNavigate()
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [isDeleting, setIsDeleting] = useState(false)

  const handleEdit = () => {
    navigate(`/edit/${movie.id}`)
  }

  const handleDeleteClick = () => {
    setShowDeleteModal(true)
  }

  const confirmDelete = async () => {
    setIsDeleting(true)
    await onDelete(movie.id)
    setShowDeleteModal(false)
  }

  const renderRating = (rating) => {
    if (!rating) return null
    return (
      <div className="flex items-center gap-1">
        {[...Array(5)].map((_, index) => (
          <Star
            key={index}
            size={14}
            className={index < rating ? 'fill-yellow-400 text-yellow-400' : 'text-gray-300'}
          />
        ))}
      </div>
    )
  }

  const formatDate = (dateString) => {
    const date = new Date(dateString)
    return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
  }

  return (
    <>
      <div className="bg-white rounded-xl shadow-md hover:shadow-xl transition-all duration-300 overflow-hidden group">
        {/* Poster Image */}
        <div className="relative h-64 bg-gradient-to-br from-gray-100 to-gray-200 overflow-hidden">
          {movie.posterImage ? (
            <img
              src={movie.posterImage}
              alt={movie.title}
              className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-300"
              onError={(e) => {
                e.target.onerror = null
                e.target.src = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjAwIiBoZWlnaHQ9IjMwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMjAwIiBoZWlnaHQ9IjMwMCIgZmlsbD0iI2UyZThmMCIvPjx0ZXh0IHg9IjUwJSIgeT0iNTAlIiBmb250LWZhbWlseT0iQXJpYWwiIGZvbnQtc2l6ZT0iMTgiIGZpbGw9IiM5Y2EzYWYiIHRleHQtYW5jaG9yPSJtaWRkbGUiIGR5PSIuM2VtIj5ObyBJbWFnZTwvdGV4dD48L3N2Zz4='
              }}
            />
          ) : (
            <div className="w-full h-full flex items-center justify-center">
              <Film className="text-gray-400" size={48} />
            </div>
          )}
          
          {/* Genre Badge */}
          {movie.genre && (
            <div className="absolute top-3 left-3 bg-black/70 backdrop-blur-sm text-white px-3 py-1 rounded-full text-xs font-medium">
              {movie.genre}
            </div>
          )}
        </div>

        {/* Content */}
        <div className="p-5">
          {/* Title */}
          <h3 className="text-lg font-bold text-gray-900 mb-2 line-clamp-2 min-h-[3.5rem]">
            {movie.title}
          </h3>

          {/* Rating */}
          <div className="mb-3">
            {renderRating(movie.rating)}
          </div>

          {/* Date */}
          {movie.createdAt && (
            <div className="flex items-center gap-2 text-sm text-gray-500 mb-4">
              <Calendar size={14} />
              <span>Added {formatDate(movie.createdAt)}</span>
            </div>
          )}

          {/* Actions */}
          <div className="flex gap-2">
            <button
              onClick={handleEdit}
              className="flex-1 inline-flex items-center justify-center gap-2 px-4 py-2 bg-primary-50 text-primary-600 rounded-lg hover:bg-primary-100 transition-colors font-medium"
            >
              <Edit size={16} />
              <span>Edit</span>
            </button>
            <button
              onClick={handleDeleteClick}
              disabled={isDeleting}
              className="inline-flex items-center justify-center gap-2 px-4 py-2 bg-red-50 text-red-600 rounded-lg hover:bg-red-100 transition-colors font-medium disabled:opacity-50"
            >
              <Trash2 size={16} />
            </button>
          </div>
        </div>
      </div>

      {/* Delete Confirmation Modal */}
      <ConfirmModal
        isOpen={showDeleteModal}
        onClose={() => setShowDeleteModal(false)}
        onConfirm={confirmDelete}
        title="Delete Movie"
        message={`Are you sure you want to delete "${movie.title}"? This action cannot be undone.`}
        confirmLabel="Delete"
        confirmClass="bg-red-500 hover:bg-red-600"
      />
    </>
  )
}

export default MovieCard
