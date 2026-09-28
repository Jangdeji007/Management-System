import { apiClient } from './client'
import type { UserSummary } from './types'

export async function listUsers(teamId?: string): Promise<UserSummary[]> {
  const { data } = await apiClient.get<UserSummary[]>('/api/users', {
    params: teamId ? { teamId } : undefined,
  })
  return data
}
