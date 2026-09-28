import axios from 'axios'
import type { ApiErrorBody } from './types'

export function getErrorMessage(error: unknown, fallback = 'Something went wrong.'): string {
  if (axios.isAxiosError<ApiErrorBody>(error)) {
    const data = error.response?.data
    if (data?.title) return data.title
    if (data?.errors) {
      const first = Object.values(data.errors)[0]?.[0]
      if (first) return first
    }
    if (error.response?.status === 401) return 'Invalid email or password.'
  }
  return fallback
}
