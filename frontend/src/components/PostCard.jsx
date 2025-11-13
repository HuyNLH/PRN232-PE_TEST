import { Link } from 'react-router-dom'
import { Calendar, Edit, Trash2, Image as ImageIcon } from 'lucide-react'

export default function PostCard({ post, onDelete }) {
  return (
    <div className="group bg-white rounded-2xl overflow-hidden card-hover border border-gray-100">
      {/* Image Container with Gradient Overlay */}
      <div className="relative h-56 bg-gradient-to-br from-primary-100 to-accent-100 overflow-hidden">
        {post.image ? (
          <>
            <img
              src={post.image}
              alt={post.name}
              className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110"
            />
            <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-black/20 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
          </>
        ) : (
          <div className="w-full h-full flex items-center justify-center">
            <ImageIcon className="w-20 h-20 text-gray-300" strokeWidth={1.5} />
          </div>
        )}
        
        {/* Badge */}
        <div className="absolute top-3 right-3">
          <span className="px-3 py-1 bg-white/90 backdrop-blur-sm rounded-full text-xs font-semibold text-primary-600 shadow-lg">
            New
          </span>
        </div>
      </div>

      {/* Content */}
      <div className="p-6">
        <h3 className="text-xl font-bold text-gray-900 mb-2 line-clamp-1 group-hover:text-primary-600 transition-colors">
          {post.name}
        </h3>
        
        <p className="text-gray-600 text-sm mb-4 line-clamp-2 leading-relaxed">
          {post.description}
        </p>

        {/* Date with Icon */}
        <div className="flex items-center gap-2 text-xs text-gray-500 mb-4 pb-4 border-b border-gray-100">
          <Calendar size={14} />
          <span>{new Date(post.createdAt).toLocaleDateString('vi-VN', { 
            year: 'numeric', 
            month: 'long', 
            day: 'numeric' 
          })}</span>
        </div>

        {/* Action Buttons */}
        <div className="flex gap-2">
          <Link
            to={`/edit/${post.id}`}
            className="flex-1 flex items-center justify-center gap-2 bg-gradient-to-r from-primary-500 to-primary-600 text-white px-4 py-2.5 rounded-xl font-medium hover:from-primary-600 hover:to-primary-700 transform hover:scale-105 transition-all duration-200 text-center shadow-lg shadow-primary-500/30"
          >
            <Edit size={18} />
            Edit
          </Link>
          <button
            onClick={() => onDelete(post.id)}
            className="flex-1 flex items-center justify-center gap-2 bg-gradient-to-r from-danger-500 to-danger-600 text-white px-4 py-2.5 rounded-xl font-medium hover:from-danger-600 hover:to-danger-700 transform hover:scale-105 transition-all duration-200 shadow-lg shadow-danger-500/30"
          >
            <Trash2 size={18} />
            Delete
          </button>
        </div>
      </div>
    </div>
  )
}
