import axios from 'axios';

// Use environment variable for API URL with fallback
const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5201';

const api = axios.create({
  baseURL: `${API_URL}/api/movies`,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Movies API
export const moviesApi = {
  // Get all movies with optional filters (search, genre, sort, pagination)
  getAll: (params = {}) => api.get('/', { params }),
  
  // Get single movie by ID
  getById: (id) => api.get(`/${id}`),
  
  // Create new movie with form data (supports file upload)
  create: (data) => api.post('/', data, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }),
  
  // Update movie with form data
  update: (id, data) => api.put(`/${id}`, data, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }),
  
  // Delete movie
  delete: (id) => api.delete(`/${id}`),
};

// Health check
export const healthCheck = () => axios.get(`${API_URL}/health`);

export default api;
