import { useEffect } from 'react'

export default function Toast({ message, type = 'success', onClose }) {
  useEffect(() => {
    const timer = setTimeout(() => {
      onClose()
    }, 3000)

    return () => clearTimeout(timer)
  }, [onClose])

  const styles = {
    success: {
      bg: 'bg-gradient-to-r from-success-500 to-success-600',
      icon: '✅'
    },
    error: {
      bg: 'bg-gradient-to-r from-danger-500 to-danger-600',
      icon: '❌'
    },
    info: {
      bg: 'bg-gradient-to-r from-primary-500 to-primary-600',
      icon: 'ℹ️'
    }
  }

  const style = styles[type] || styles.success

  return (
    <div className="fixed top-6 right-6 z-50 animate-slide-in">
      <div className={`${style.bg} text-white px-6 py-4 rounded-2xl shadow-2xl flex items-center gap-3 min-w-[300px] backdrop-blur-lg border border-white/20`}>
        <span className="text-2xl">{style.icon}</span>
        <span className="flex-1 font-medium">{message}</span>
        <button
          onClick={onClose}
          className="text-white/80 hover:text-white font-bold text-2xl transition-colors ml-2"
        >
          ×
        </button>
      </div>
    </div>
  )
}
