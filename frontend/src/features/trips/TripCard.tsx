import { Link } from 'react-router-dom'
import { MapPin, Image, Building2 } from 'lucide-react'
import type { TripSummary } from '../../types/api'
import { format } from 'date-fns'

interface Props {
  trip: TripSummary
}

export function TripCard({ trip }: Props) {
  const start = format(new Date(trip.startDate), 'MMM d')
  const end = format(new Date(trip.endDate), 'MMM d, yyyy')

  return (
    <Link to={`/trips/${trip.id}`} className="block group">
      <div className="bg-white rounded-2xl overflow-hidden shadow-sm border border-warm-100 hover:shadow-md transition-shadow">
        {trip.coverPhoto ? (
          <img
            src={trip.coverPhoto.thumbUrl}
            alt={trip.title}
            className="w-full h-40 object-cover group-hover:scale-[1.02] transition-transform duration-300"
          />
        ) : (
          <div className="w-full h-40 bg-gradient-to-br from-warm-100 to-sage-100 flex items-center justify-center text-warm-300">
            <Building2 size={40} strokeWidth={1} />
          </div>
        )}
        <div className="p-4">
          <h3 className="font-semibold text-warm-900 truncate">{trip.title}</h3>
          <p className="text-sm text-warm-500 mt-0.5 flex items-center gap-1">
            <MapPin size={12} className="flex-shrink-0" />
            {trip.city.name}, {trip.city.country}
          </p>
          <p className="text-xs text-warm-400 mt-1 flex items-center gap-1">
            {start} – {end}
            {trip.photoCount > 0 && (
              <span className="ml-1 text-warm-300 flex items-center gap-0.5">
                · <Image size={10} className="inline" /> {trip.photoCount}
              </span>
            )}
          </p>
        </div>
      </div>
    </Link>
  )
}
