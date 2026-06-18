export interface MeResponse {
  id: string
  email: string
  displayName: string
  avatarUrl: string | null
  coupleId: string
}

export interface CityCandidate {
  placeId: string
  name: string
  country: string
  displayName: string
  lat: number
  lng: number
}

export interface CitySummary {
  id: string
  name: string
  country: string
  lat: number
  lng: number
}

export interface PhotoSummary {
  id: string
  url: string
  thumbUrl: string
  width: number
  height: number
  caption: string | null
  orderIndex: number
}

export interface TripSummary {
  id: string
  title: string
  note: string | null
  startDate: string
  endDate: string
  city: CitySummary
  coverPhoto: PhotoSummary | null
  photoCount: number
  createdAt: string
}

export interface TripDetail extends TripSummary {
  photos: PhotoSummary[]
  updatedAt: string
}

export interface TimelineResponse {
  trips: TripSummary[]
  nextCursor: string | null
}

export interface CityPin {
  cityId: string
  name: string
  country: string
  lat: number
  lng: number
  tripCount: number
}

export interface CreateTripRequest {
  title: string
  note?: string
  startDate: string
  endDate: string
  clientUuid: string
  cityId?: string
  cityPlaceId?: string
  cityName?: string
  cityCountry?: string
  cityLat?: number
  cityLng?: number
  cityGeocodeSource?: string
}

export interface PresignedUploadResponse {
  photoId: string
  uploadUrl: string
  thumbUploadUrl: string
}
