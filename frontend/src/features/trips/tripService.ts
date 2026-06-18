import { api } from '../../lib/api'
import type {
  CreateTripRequest,
  TimelineResponse,
  TripDetail,
} from '../../types/api'

export const tripService = {
  create: (body: CreateTripRequest) => api.post<TripDetail>('/trips', body),
  timeline: (cursor?: string) =>
    api.get<TimelineResponse>(`/trips${cursor ? `?cursor=${encodeURIComponent(cursor)}` : ''}`),
  getById: (id: string) => api.get<TripDetail>(`/trips/${id}`),
  update: (id: string, body: Partial<CreateTripRequest>) =>
    api.put<TripDetail>(`/trips/${id}`, body),
  delete: (id: string) => api.delete<void>(`/trips/${id}`),
}
