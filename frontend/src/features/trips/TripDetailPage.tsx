import { useParams, Link, useNavigate } from 'react-router-dom'
import { useTrip, useDeleteTrip } from './useTrips'
import { format } from 'date-fns'

export function TripDetailPage() {
  const { id } = useParams<{ id: string }>()
  const { data: trip, isLoading } = useTrip(id!)
  const deleteTrip = useDeleteTrip()
  const navigate = useNavigate()

  const handleDelete = async () => {
    if (!confirm('Delete this trip? This cannot be undone.')) return
    await deleteTrip.mutateAsync(id!)
    navigate('/timeline')
  }

  if (isLoading) {
    return <div className="max-w-lg mx-auto px-4 py-10 text-center text-warm-400 animate-pulse text-4xl">🗺️</div>
  }

  if (!trip) return <div className="max-w-lg mx-auto px-4 py-10 text-center text-warm-500">Trip not found.</div>

  const start = format(new Date(trip.startDate), 'MMMM d')
  const end = format(new Date(trip.endDate), 'MMMM d, yyyy')

  return (
    <div className="max-w-lg mx-auto px-4 py-6">
      <Link to="/timeline" className="text-warm-500 text-sm hover:text-warm-700 transition-colors mb-4 inline-block">
        ← Back
      </Link>

      <div className="bg-white rounded-2xl overflow-hidden shadow-sm border border-warm-100">
        {trip.coverPhoto && (
          <img src={trip.coverPhoto.url} alt={trip.title} className="w-full h-56 object-cover" />
        )}
        <div className="p-5">
          <h1 className="text-2xl font-bold text-warm-900" style={{ fontFamily: 'Georgia, serif' }}>
            {trip.title}
          </h1>
          <p className="text-warm-500 mt-1">
            📍 {trip.city.name}, {trip.city.country}
          </p>
          <p className="text-sm text-warm-400 mt-0.5">{start} – {end}</p>
          {trip.note && <p className="mt-4 text-warm-700 text-sm leading-relaxed">{trip.note}</p>}
        </div>
      </div>

      {trip.photos.length > 0 && (
        <div className="mt-6">
          <h2 className="font-semibold text-warm-800 mb-3">Photos</h2>
          <div className="grid grid-cols-3 gap-2">
            {trip.photos.map(photo => (
              <img
                key={photo.id}
                src={photo.thumbUrl}
                alt={photo.caption ?? ''}
                className="w-full aspect-square object-cover rounded-xl"
              />
            ))}
          </div>
        </div>
      )}

      <div className="mt-8 flex justify-end">
        <button
          onClick={handleDelete}
          disabled={deleteTrip.isPending}
          className="text-red-400 hover:text-red-600 text-sm transition-colors"
        >
          Delete trip
        </button>
      </div>
    </div>
  )
}
