import { apiClient } from './client'
import type { NotificationItem } from './types'

export async function listNotifications(unreadOnly?: boolean): Promise<NotificationItem[]> {
  const { data } = await apiClient.get<NotificationItem[]>('/api/notifications', {
    params: unreadOnly === undefined ? undefined : { unreadOnly },
  })
  return data
}

export async function markNotificationRead(id: string): Promise<NotificationItem> {
  const { data } = await apiClient.patch<NotificationItem>(`/api/notifications/${id}/read`)
  return data
}
