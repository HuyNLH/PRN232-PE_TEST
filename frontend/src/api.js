import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || 'https://localhost:7123';

const api = axios.create({
  baseURL: `${API_URL}/api`,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Posts API
export const postsApi = {
  // Get all posts with optional filters
  getAll: (params = {}) => api.get('/posts', { params }),
  
  // Get single post by ID
  getById: (id) => api.get(`/posts/${id}`),
  
  // Create new post with JSON data
  create: (data) => api.post('/posts', data, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }),
  
  // Update post
  update: (id, data) => api.put(`/posts/${id}`, data, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }),
  
  // Delete post
  delete: (id) => api.delete(`/posts/${id}`),
};

// Health check
export const healthCheck = () => api.get('/', { baseURL: API_URL });

export default api;
