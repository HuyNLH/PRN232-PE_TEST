export default function ConfirmModal({ title, message, onConfirm, onCancel }) {
  return (
    <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 animate-fade-in p-4">
      <div className="bg-white rounded-3xl p-8 max-w-md w-full mx-4 shadow-2xl transform animate-bounce-subtle">
        {/* Icon */}
        <div className="w-16 h-16 bg-gradient-to-br from-danger-100 to-danger-200 rounded-full flex items-center justify-center mx-auto mb-4">
          <span className="text-4xl">⚠️</span>
        </div>

        {/* Title */}
        <h3 className="text-2xl font-bold text-gray-900 mb-3 text-center">{title}</h3>
        
        {/* Message */}
        <p className="text-gray-600 mb-8 text-center leading-relaxed">{message}</p>
        
        {/* Buttons */}
        <div className="flex gap-3">
          <button
            onClick={onCancel}
            className="flex-1 bg-gray-100 text-gray-700 font-semibold py-3 px-6 rounded-xl hover:bg-gray-200 transition-all duration-200"
          >
            Cancel
          </button>
          <button
            onClick={onConfirm}
            className="flex-1 bg-gradient-to-r from-danger-500 to-danger-600 text-white font-semibold py-3 px-6 rounded-xl hover:from-danger-600 hover:to-danger-700 transform hover:scale-105 transition-all duration-200 shadow-lg shadow-danger-500/30"
          >
            Yes, Delete
          </button>
        </div>
      </div>
    </div>
  )
}
