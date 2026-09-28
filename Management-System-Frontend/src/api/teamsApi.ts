import { apiClient } from './client'
import type { TeamListItem } from './types'

export async function listTeams(): Promise<TeamListItem[]> {
  const { data } = await apiClient.get<TeamListItem[]>('/api/teams')
  return data
}

export async function createTeam(name: string, description?: string): Promise<TeamListItem> {
  const { data } = await apiClient.post<TeamListItem>('/api/teams', { name, description })
  return data
}

export async function updateTeam(
  id: string,
  name: string,
  description?: string,
): Promise<TeamListItem> {
  const { data } = await apiClient.put<TeamListItem>(`/api/teams/${id}`, { name, description })
  return data
}

export async function addTeamMember(teamId: string, userId: string): Promise<TeamListItem> {
  const { data } = await apiClient.post<TeamListItem>(`/api/teams/${teamId}/members`, { userId })
  return data
}

export async function removeTeamMember(teamId: string, userId: string): Promise<TeamListItem> {
  const { data } = await apiClient.delete<TeamListItem>(
    `/api/teams/${teamId}/members/${userId}`,
  )
  return data
}
