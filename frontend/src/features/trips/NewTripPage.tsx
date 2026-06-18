import { useState, useEffect, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import { MapPin } from 'lucide-react'
import { useCreateTrip } from './useTrips'
import { api } from '../../lib/api'
import type { CityCandidate } from '../../types/api'
import { saveFormDraft, loadFormDraft, clearFormDraft, DRAFT_KEYS } from '../../lib/localStorage'

interface Draft {
  title: string
  note: string
  startDate: string
  endDate: string
}

export function NewTripPage() {
  const navigate = useNavigate()
  const createTrip = useCreateTrip()

  const draft = loadFormDraft<Draft>(DRAFT_KEYS.newTrip)
  const [title, setTitle] = useState(draft?.title ?? '')
  const [note, setNote] = useState(draft?.note ?? '')
  const [startDate, setStartDate] = useState(draft?.startDate ?? '')
  const [endDate, setEndDate] = useState(draft?.endDate ?? '')

  const [cityQuery, setCityQuery] = useState('')
  const [candidates, setCandidates] = useState<CityCandidate[]>([])
  const [selectedCity, setSelectedCity] = useState<CityCandidate | null>(null)
  const [searching, setSearching] = useState(false)
  const [error, setError] = useState('')

  const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null)

  useEffect(() => {
    saveFormDraft(DRAFT_KEYS.newTrip, { title, note, startDate, endDate })
  }, [title, note, startDate, endDate])

  useEffect(() => {
    if (cityQuery.length < 2) { setCandidates([]); return }
    if (debounceRef.current) clearTimeout(debounceRef.current)
    debounceRef.current = setTimeout(async () => {
      setSearching(true)
      try {
        const results = await api.get<CityCandidate[]>(`/cities/search?q=${encodeURIComponent(cityQuery)}`)
        setCandidates(results)
      } finally {
        setSearching(false)
      }
    }, 400)
  }, [cityQuery])

  const submit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!selectedCity) { setError('Please select a city'); return }
    setError('')

    try {
      const trip = await createTrip.mutateAsync({
        title,
        note: note || undefined,
        startDate,
        endDate,
        clientUuid: crypto.randomUUID(),
        cityPlaceId: selectedCity.placeId,
        cityName: selectedCity.name,
        cityCountry: selectedCity.country,
        cityLat: selectedCity.lat,
        cityLng: selectedCity.lng,
        cityGeocodeSource: 'nominatim',
      })
      clearFormDraft(DRAFT_KEYS.newTrip)
      navigate(`/trips/${trip.id}`)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create trip')
    }
  }

  return (
    <div className="max-w-lg mx-auto px-4 py-6">
      <h1 className="font-display text-2xl font-semibold text-warm-800 mb-6">
        Log a trip
      </h1>

      <form onSubmit={submit} className="space-y-5">
        <div>
          <label className="block text-sm font-medium text-warm-700 mb-1">Title</label>
          <input
            value={title}
            onChange={e => setTitle(e.target.value)}
            required
            placeholder="Phú Yên getaway"
            className="w-full rounded-xl border border-warm-200 px-3 py-2 text-sm outline-none focus:border-warm-400 focus:ring-2 focus:ring-warm-100"
          />
        </div>

        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className="block text-sm font-medium text-warm-700 mb-1">From</label>
            <input
              type="date"
              value={startDate}
              onChange={e => setStartDate(e.target.value)}
              required
              className="w-full rounded-xl border border-warm-200 px-3 py-2 text-sm outline-none focus:border-warm-400"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-warm-700 mb-1">To</label>
            <input
              type="date"
              value={endDate}
              onChange={e => setEndDate(e.target.value)}
              required
              min={startDate}
              className="w-full rounded-xl border border-warm-200 px-3 py-2 text-sm outline-none focus:border-warm-400"
            />
          </div>
        </div>

        <div className="relative">
          <label className="block text-sm font-medium text-warm-700 mb-1">City</label>
          {selectedCity ? (
            <div className="flex items-center justify-between bg-sage-50 border border-sage-200 rounded-xl px-3 py-2">
              <span className="text-sm text-sage-800 flex items-center gap-1"><MapPin size={12} className="flex-shrink-0" />{selectedCity.name}, {selectedCity.country}</span>
              <button
                type="button"
                onClick={() => { setSelectedCity(null); setCityQuery(''); setCandidates([]) }}
                className="text-sage-400 hover:text-sage-600 text-xs ml-2"
              >
                ×
              </button>
            </div>
          ) : (
            <>
              <input
                value={cityQuery}
                onChange={e => setCityQuery(e.target.value)}
                placeholder="Search for a city…"
                className="w-full rounded-xl border border-warm-200 px-3 py-2 text-sm outline-none focus:border-warm-400 focus:ring-2 focus:ring-warm-100"
              />
              {(candidates.length > 0 || searching) && (
                <div className="absolute top-full left-0 right-0 mt-1 bg-white border border-warm-100 rounded-xl shadow-lg z-10 overflow-hidden">
                  {searching ? (
                    <div className="px-3 py-2 text-sm text-warm-400">Searching…</div>
                  ) : (
                    candidates.map(c => (
                      <button
                        key={c.placeId}
                        type="button"
                        onClick={() => { setSelectedCity(c); setCandidates([]); setCityQuery('') }}
                        className="w-full text-left px-3 py-2.5 text-sm hover:bg-warm-50 transition-colors"
                      >
                        <span className="font-medium text-warm-900">{c.name}</span>
                        <span className="text-warm-400 ml-1 text-xs">{c.country}</span>
                        <div className="text-xs text-warm-300 truncate">{c.displayName}</div>
                      </button>
                    ))
                  )}
                </div>
              )}
            </>
          )}
        </div>

        <div>
          <label className="block text-sm font-medium text-warm-700 mb-1">Notes <span className="text-warm-300">(optional)</span></label>
          <textarea
            value={note}
            onChange={e => setNote(e.target.value)}
            rows={3}
            placeholder="Anything you want to remember…"
            className="w-full rounded-xl border border-warm-200 px-3 py-2 text-sm outline-none focus:border-warm-400 resize-none"
          />
        </div>

        {error && <p className="text-red-500 text-sm">{error}</p>}

        <div className="flex gap-3">
          <button
            type="button"
            onClick={() => navigate(-1)}
            className="flex-1 border border-warm-200 text-warm-600 rounded-xl py-2.5 text-sm font-medium hover:bg-warm-50 transition-colors"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={createTrip.isPending}
            className="flex-1 bg-warm-500 hover:bg-warm-600 text-white rounded-xl py-2.5 text-sm font-medium transition-colors disabled:opacity-60"
          >
            {createTrip.isPending ? 'Saving…' : 'Save trip'}
          </button>
        </div>
      </form>
    </div>
  )
}
