import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { authService } from './authService'

export function useMe() {
  return useQuery({
    queryKey: ['me'],
    queryFn: authService.me,
    retry: false,
  })
}

export function useLogin() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ email, password }: { email: string; password: string }) =>
      authService.login(email, password),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['me'] }),
  })
}

export function useLogout() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: authService.logout,
    onSuccess: () => qc.clear(),
  })
}
