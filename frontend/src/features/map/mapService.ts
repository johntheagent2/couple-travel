import { api } from '../../lib/api'
import type { CityPin } from '../../types/api'

export const mapService = {
  cityPins: () => api.get<CityPin[]>('/map/cities'),
}
