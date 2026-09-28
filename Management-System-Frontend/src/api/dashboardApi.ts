import { apiClient } from './client'
import { buildTaskQueryParams } from './query'
import type { DashboardSummary, TaskQueryParams } from './types'

export async function getDashboardSummary(
  params: TaskQueryParams = {},
): Promise<DashboardSummary> {
  const { data } = await apiClient.get<DashboardSummary>('/api/dashboard/summary', {
    params: buildTaskQueryParams(params),
  })
  return data
}
