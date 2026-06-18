import { api } from '../../lib/api'
import type { MeResponse } from '../../types/api'

export const authService = {
  login: (email: string, password: string) =>
    api.post<MeResponse>('/auth/login', { email, password }),
  logout: () => api.post<void>('/auth/logout', {}),
  me: () => api.get<MeResponse>('/auth/me'),
}
