import { useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import maplibregl from 'maplibre-gl'
import 'maplibre-gl/dist/maplibre-gl.css'
import { mapService } from './mapService'
import type { CityPin, TripSummary } from '../../types/api'
import { api } from '../../lib/api'
import type { TimelineResponse } from '../../types/api'

export function MapPage() {
  const mapRef = useRef<HTMLDivElement>(null)
  const mapInstance = useRef<maplibregl.Map | null>(null)
  const markers = useRef<maplibregl.Marker[]>([])
  const navigate = useNavigate()

  const [selectedCity, setSelectedCity] = useState<{ pin: CityPin; trips: TripSummary[] } | null>(null)

  const { data: pins } = useQuery({
    queryKey: ['map', 'cities'],
    queryFn: mapService.cityPins,
  })

  useEffect(() => {
    if (!mapRef.current) return
    const map = new maplibregl.Map({
      container: mapRef.current,
      style: 'https://tiles.openfreemap.org/styles/bright',
      center: [106.66, 16.0],
      zoom: 4,
    })
    map.addControl(new maplibregl.NavigationControl(), 'top-right')
    mapInstance.current = map
    return () => map.remove()
  }, [])

  useEffect(() => {
    const map = mapInstance.current
    if (!map || !pins) return

    markers.current.forEach(m => m.remove())
    markers.current = []

    pins.forEach(pin => {
      const el = document.createElement('div')
      el.className = 'city-pin'
      el.innerHTML = `
        <div style="
          background: #dc8f30;
          color: white;
          border-radius: 9999px;
          padding: 4px 10px;
          font-size: 12px;
          font-weight: 600;
          white-space: nowrap;
          box-shadow: 0 2px 8px rgba(0,0,0,0.2);
          cursor: pointer;
          border: 2px solid white;
        ">📍 ${pin.name}</div>
      `
      el.addEventListener('click', async () => {
        const data = await api.get<TimelineResponse>(`/trips?limit=50`)
        const cityTrips = data.trips.filter(t => t.city.id === pin.cityId)
        setSelectedCity({ pin, trips: cityTrips })
      })

      const marker = new maplibregl.Marker({ element: el })
        .setLngLat([pin.lng, pin.lat])
        .addTo(map)
      markers.current.push(marker)
    })
  }, [pins])

  return (
    <div className="relative h-full">
      <div ref={mapRef} className="w-full h-full" />

      {selectedCity && (
        <div className="absolute bottom-0 left-0 right-0 bg-white rounded-t-3xl shadow-2xl p-5 max-h-[60vh] overflow-y-auto">
          <div className="flex items-center justify-between mb-4">
            <div>
              <h2 className="font-bold text-warm-900 text-lg">{selectedCity.pin.name}</h2>
              <p className="text-sm text-warm-500">{selectedCity.pin.country} · {selectedCity.pin.tripCount} trip{selectedCity.pin.tripCount !== 1 ? 's' : ''}</p>
            </div>
            <button
              onClick={() => setSelectedCity(null)}
              className="text-warm-300 hover:text-warm-600 text-2xl"
            >×</button>
          </div>
          <div className="space-y-3">
            {selectedCity.trips.map(trip => (
              <button
                key={trip.id}
                onClick={() => navigate(`/trips/${trip.id}`)}
                className="w-full text-left flex items-center gap-3 p-3 bg-warm-50 rounded-xl hover:bg-warm-100 transition-colors"
              >
                {trip.coverPhoto ? (
                  <img src={trip.coverPhoto.thumbUrl} alt="" className="w-14 h-14 rounded-xl object-cover flex-shrink-0" />
                ) : (
                  <div className="w-14 h-14 rounded-xl bg-warm-200 flex items-center justify-center text-xl flex-shrink-0">🏙️</div>
                )}
                <div className="min-w-0">
                  <p className="font-medium text-warm-900 text-sm truncate">{trip.title}</p>
                  <p className="text-xs text-warm-400">{trip.startDate} – {trip.endDate}</p>
                </div>
              </button>
            ))}
          </div>
        </div>
      )}
    </div>
  )
}
