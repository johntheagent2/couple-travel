import { createBrowserRouter } from 'react-router-dom'
import { AuthGuard } from '../features/auth/AuthGuard'
import { LoginPage } from '../features/auth/LoginPage'
import { Layout } from '../components/Layout'
import { MapPage } from '../features/map/MapPage'
import { TimelinePage } from '../features/trips/TimelinePage'
import { TripDetailPage } from '../features/trips/TripDetailPage'
import { NewTripPage } from '../features/trips/NewTripPage'

function AppShell({ children }: { children: React.ReactNode }) {
  return (
    <AuthGuard>
      <Layout>{children}</Layout>
    </AuthGuard>
  )
}

export const router = createBrowserRouter([
  { path: '/login', element: <LoginPage /> },
  { path: '/', element: <AppShell><MapPage /></AppShell> },
  { path: '/timeline', element: <AppShell><TimelinePage /></AppShell> },
  { path: '/trips/new', element: <AppShell><NewTripPage /></AppShell> },
  { path: '/trips/:id', element: <AppShell><TripDetailPage /></AppShell> },
])
