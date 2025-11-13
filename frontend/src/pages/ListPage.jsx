import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { Search, ArrowUpDown, Loader2, AlertCircle, Inbox } from 'lucide-react'
import { postService } from '../services/api'
import PostCard from '../components/PostCard'
import ConfirmModal from '../components/ConfirmModal'
import Toast from '../components/Toast'

export default function ListPage() {
  const [posts, setPosts] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [search, setSearch] = useState('')
  const [sort, setSort] = useState('')
  const [deleteId, setDeleteId] = useState(null)
  const [toast, setToast] = useState(null)

  const fetchPosts = async () => {
    try {
      setLoading(true)
      const params = {}
      if (search) params.q = search
      if (sort) params.sort = sort
      
      const response = await postService.getAll(params)
      setPosts(response.data)
      setError(null)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchPosts()
  }, [search, sort])

  const handleDelete = async () => {
    try {
      await postService.delete(deleteId)
      setDeleteId(null)
      setToast({ message: 'Post deleted successfully!', type: 'success' })
      fetchPosts()
    } catch (err) {
      setError(err.message)
      setToast({ message: 'Failed to delete post', type: 'error' })
    }
  }

  if (loading) {
    return (
      <div className="flex flex-col items-center justify-center py-20">
        <Loader2 className="w-16 h-16 text-primary-600 animate-spin" />
        <p className="mt-4 text-gray-600 font-medium">Loading amazing posts...</p>
      </div>
    )
  }

  if (error) {
    return (
      <div className="bg-gradient-to-r from-danger-50 to-danger-100 border-l-4 border-danger-500 text-danger-800 px-6 py-4 rounded-xl shadow-soft">
        <div className="flex items-center gap-3">
          <AlertCircle className="text-danger-600" size={24} />
          <div>
            <h3 className="font-bold">Oops! Something went wrong</h3>
            <p className="text-sm">{error}</p>
          </div>
        </div>
      </div>
    )
  }

  return (
    <div>
      {/* Header Section */}
      <div className="mb-8">
        <h1 className="text-4xl font-bold text-gray-900 mb-2">
          Discover <span className="text-gradient">Amazing Posts</span>
        </h1>
        <p className="text-gray-600">Explore, search, and manage your content collection</p>
      </div>

      {/* Search & Filter Bar */}
      <div className="mb-8 bg-white/80 backdrop-blur-lg p-6 rounded-2xl shadow-soft border border-gray-100">
        <div className="flex flex-col md:flex-row gap-4">
          {/* Search Input */}
          <div className="flex-1 relative">
            <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
              <Search className="text-gray-400" size={20} />
            </div>
            <input
              type="text"
              placeholder="Search posts by name or description..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="w-full pl-12 pr-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent bg-white transition-all"
            />
          </div>

          {/* Sort Dropdown */}
          <div className="relative">
            <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
              <ArrowUpDown className="text-gray-400" size={20} />
            </div>
            <select
              value={sort}
              onChange={(e) => setSort(e.target.value)}
              className="w-full md:w-64 pl-12 pr-10 py-3 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent bg-white cursor-pointer appearance-none transition-all"
            >
              <option value="">Sort by...</option>
              <option value="name_asc">Name (A → Z)</option>
              <option value="name_desc">Name (Z → A)</option>
              <option value="date_desc">Newest First</option>
              <option value="date_asc">Oldest First</option>
            </select>
          </div>
        </div>

        {/* Active Filters */}
        {(search || sort) && (
          <div className="mt-4 flex items-center gap-2 flex-wrap">
            <span className="text-sm text-gray-600">Active filters:</span>
            {search && (
              <span className="px-3 py-1 bg-primary-100 text-primary-700 rounded-full text-sm font-medium">
                Search: "{search}"
              </span>
            )}
            {sort && (
              <span className="px-3 py-1 bg-accent-100 text-accent-700 rounded-full text-sm font-medium">
                Sort: {sort === 'name_asc' ? 'A → Z' : 'Z → A'}
              </span>
            )}
            <button
              onClick={() => {
                setSearch('')
                setSort('')
              }}
              className="px-3 py-1 bg-gray-100 text-gray-700 rounded-full text-sm font-medium hover:bg-gray-200 transition-colors"
            >
              ✕ Clear all
            </button>
          </div>
        )}
      </div>

      {/* Posts Grid or Empty State */}
      {posts.length === 0 ? (
        <div className="text-center py-20 bg-white/50 backdrop-blur-sm rounded-2xl border-2 border-dashed border-gray-300">
          <Inbox className="w-24 h-24 mx-auto text-gray-300 mb-4" />
          <h3 className="text-2xl font-bold text-gray-900 mb-2">No posts found</h3>
          <p className="text-gray-600 mb-6">
            {search ? `No results for "${search}"` : "Start by creating your first amazing post!"}
          </p>
          <Link
            to="/create"
            className="inline-flex items-center gap-2 bg-gradient-to-r from-primary-500 to-primary-600 text-white px-6 py-3 rounded-xl font-medium hover:from-primary-600 hover:to-primary-700 transform hover:scale-105 transition-all duration-200 shadow-lg shadow-primary-500/30"
          >
            <PlusCircle size={20} />
            Create Your First Post
          </Link>
        </div>
      ) : (
        <>
          {/* Results Count */}
          <div className="mb-4 flex items-center justify-between">
            <p className="text-sm text-gray-600">
              Found <span className="font-bold text-primary-600">{posts.length}</span> post{posts.length !== 1 ? 's' : ''}
            </p>
          </div>

          {/* Posts Grid */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {posts.map((post) => (
              <PostCard
                key={post.id}
                post={post}
                onDelete={(id) => setDeleteId(id)}
              />
            ))}
          </div>
        </>
      )}

      {deleteId && (
        <ConfirmModal
          title="Delete Post"
          message="Are you sure you want to delete this post? This action cannot be undone."
          onConfirm={handleDelete}
          onCancel={() => setDeleteId(null)}
        />
      )}

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
