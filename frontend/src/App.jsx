import { BrowserRouter as Router, Routes, Route, Link, useLocation } from 'react-router-dom'
import { Film, Home, PlusCircle } from 'lucide-react'
import MovieList from './pages/MovieList'
import AddMovie from './pages/AddMovie'
import EditMovie from './pages/EditMovie'

function NavLink({ to, children, icon }) {
  const location = useLocation()
  const isActive = location.pathname === to
  
  return (
    <Link
      to={to}
      className={`inline-flex items-center gap-2 px-4 py-2 rounded-lg font-medium transition-all duration-200 ${
        isActive
          ? 'bg-primary-500 text-white shadow-lg shadow-primary-500/30'
          : 'text-gray-600 hover:text-primary-600 hover:bg-white/50'
      }`}
    >
      {icon && <span className="text-xl">{icon}</span>}
      {children}
    </Link>
  )
}

function App() {
  return (
    <Router>
      <div className="min-h-screen">
        {/* Modern Navigation */}
        <nav className="bg-white/80 backdrop-blur-lg border-b border-gray-200/50 sticky top-0 z-50 shadow-sm">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div className="flex items-center justify-between h-20">
              {/* Logo */}
              <Link to="/" className="flex items-center gap-3 group">
                <div className="w-10 h-10 bg-gradient-to-br from-primary-500 to-accent-500 rounded-xl flex items-center justify-center transform group-hover:rotate-12 transition-transform duration-300">
                  <Film className="text-white" size={20} />
                </div>
                <div>
                  <h1 className="text-xl font-bold text-gradient">Movie Watchlist</h1>
                  <p className="text-xs text-gray-500">Track & Discover</p>
                </div>
              </Link>

              {/* Navigation Links */}
              <div className="flex items-center gap-2">
                <NavLink to="/" icon={<Home size={18} />}>All Movies</NavLink>
                <NavLink to="/add" icon={<PlusCircle size={18} />}>Add Movie</NavLink>
              </div>
            </div>
          </div>
        </nav>

        {/* Main Content with Animation */}
        <main className="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8 animate-fade-in">
          <Routes>
            <Route path="/" element={<MovieList />} />
            <Route path="/add" element={<AddMovie />} />
            <Route path="/edit/:id" element={<EditMovie />} />
          </Routes>
        </main>

        {/* Footer */}
        <footer className="mt-20 py-8 bg-white/50 backdrop-blur-sm border-t border-gray-200/50">
          <div className="max-w-7xl mx-auto px-4 text-center">
            <p className="text-gray-600 text-sm">
              Made with <span className="text-red-500">♥</span> using React + ASP.NET Core
            </p>
          </div>
        </footer>
      </div>
    </Router>
  )
}

export default App
