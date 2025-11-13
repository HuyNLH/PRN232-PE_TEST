import axios from 'axios'

const API_URL = import.meta.env.VITE_API_URL || 'https://localhost:7001'

const api = axios.create({
  baseURL: `${API_URL}/api`,
  headers: {
    'Content-Type': 'application/json',
  },
})

export const postService = {
  // Get all posts with optional search, sort, and pagination
  getAll: (params = {}) => {
    return api.get('/posts', { params })
  },

  // Get single post by ID
  getById: (id) => {
    return api.get(`/posts/${id}`)
  },

  // Create new post (multipart/form-data)
  create: (formData) => {
    return api.post('/posts', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
  },

  // Update post (multipart/form-data)
  update: (id, formData) => {
    return api.put(`/posts/${id}`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
  },

  // Delete post
  delete: (id) => {
    return api.delete(`/posts/${id}`)
  },
}

export default api
