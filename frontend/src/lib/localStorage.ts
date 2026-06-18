export function saveFormDraft<T>(key: string, data: T): void {
  try {
    localStorage.setItem(key, JSON.stringify(data))
  } catch {}
}

export function loadFormDraft<T>(key: string): T | null {
  try {
    const raw = localStorage.getItem(key)
    return raw ? (JSON.parse(raw) as T) : null
  } catch {
    return null
  }
}

export function clearFormDraft(key: string): void {
  localStorage.removeItem(key)
}

export const DRAFT_KEYS = {
  newTrip: 'draft:new-trip',
} as const
