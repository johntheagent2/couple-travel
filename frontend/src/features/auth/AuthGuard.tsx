import { Navigate } from 'react-router-dom'
import { useMe } from './useAuth'

export function AuthGuard({ children }: { children: React.ReactNode }) {
  const { data, isLoading, isError } = useMe()

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-warm-50">
        <div className="text-warm-400 text-4xl animate-pulse">🗺️</div>
      </div>
    )
  }

  if (isError || !data) {
    return <Navigate to="/login" replace />
  }

  return <>{children}</>
}
