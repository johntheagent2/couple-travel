import { Link, useLocation } from 'react-router-dom'
import { useLogout } from '../features/auth/useAuth'

export function Layout({ children }: { children: React.ReactNode }) {
  const location = useLocation()
  const logout = useLogout()
  const isMap = location.pathname === '/'

  return (
    <div className="flex flex-col h-full">
      <header className="flex items-center justify-between px-4 py-3 bg-white border-b border-warm-100 flex-shrink-0">
        <Link to="/" className="font-bold text-warm-800" style={{ fontFamily: 'Georgia, serif' }}>
          🗺️ Our Adventures
        </Link>
        <button
          onClick={() => logout.mutate()}
          className="text-xs text-warm-400 hover:text-warm-600 transition-colors"
        >
          Sign out
        </button>
      </header>

      <main className={`flex-1 overflow-auto ${isMap ? 'overflow-hidden' : ''}`}>
        {children}
      </main>

      <nav className="flex border-t border-warm-100 bg-white flex-shrink-0">
        <NavItem to="/" icon="🗺️" label="Map" active={location.pathname === '/'} />
        <NavItem to="/timeline" icon="📖" label="Timeline" active={location.pathname.startsWith('/timeline') || location.pathname.startsWith('/trips')} />
        <NavItem to="/trips/new" icon="➕" label="New trip" active={location.pathname === '/trips/new'} />
      </nav>
    </div>
  )
}

function NavItem({ to, icon, label, active }: { to: string; icon: string; label: string; active: boolean }) {
  return (
    <Link
      to={to}
      className={`flex-1 flex flex-col items-center py-2 text-xs transition-colors ${
        active ? 'text-warm-600 font-semibold' : 'text-warm-400 hover:text-warm-600'
      }`}
    >
      <span className="text-xl">{icon}</span>
      <span>{label}</span>
    </Link>
  )
}
