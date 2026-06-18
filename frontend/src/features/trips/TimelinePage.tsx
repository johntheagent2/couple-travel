import { Link } from 'react-router-dom'
import { useTimeline } from './useTrips'
import { TripCard } from './TripCard'

export function TimelinePage() {
  const { data, isLoading, fetchNextPage, hasNextPage, isFetchingNextPage } = useTimeline()

  const trips = data?.pages.flatMap(p => p.trips) ?? []

  return (
    <div className="max-w-lg mx-auto px-4 py-6">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-warm-800" style={{ fontFamily: 'Georgia, serif' }}>
          Our Trips
        </h1>
        <Link
          to="/trips/new"
          className="bg-warm-500 hover:bg-warm-600 text-white rounded-full px-4 py-2 text-sm font-medium transition-colors"
        >
          + New trip
        </Link>
      </div>

      {isLoading ? (
        <div className="space-y-4">
          {[1, 2, 3].map(i => (
            <div key={i} className="bg-white rounded-2xl h-52 animate-pulse border border-warm-100" />
          ))}
        </div>
      ) : trips.length === 0 ? (
        <div className="text-center py-20 text-warm-400">
          <div className="text-6xl mb-4">✨</div>
          <p className="text-lg font-medium text-warm-700">Your map is waiting for its first adventure</p>
          <p className="text-sm mt-2">Log your first trip to get started.</p>
          <Link
            to="/trips/new"
            className="mt-6 inline-block bg-warm-500 text-white rounded-full px-6 py-2.5 text-sm font-medium hover:bg-warm-600 transition-colors"
          >
            Log a trip
          </Link>
        </div>
      ) : (
        <div className="space-y-4">
          {trips.map(trip => <TripCard key={trip.id} trip={trip} />)}
          {hasNextPage && (
            <button
              onClick={() => fetchNextPage()}
              disabled={isFetchingNextPage}
              className="w-full py-3 text-warm-500 text-sm hover:text-warm-700 transition-colors"
            >
              {isFetchingNextPage ? 'Loading…' : 'Load more'}
            </button>
          )}
        </div>
      )}
    </div>
  )
}
