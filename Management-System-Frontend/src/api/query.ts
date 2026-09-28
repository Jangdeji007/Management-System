import type { TaskQueryParams } from './types'

export function buildTaskQueryParams(params: TaskQueryParams): Record<string, string> {
  const query: Record<string, string> = {}
  if (params.status) query.status = params.status
  if (params.priority) query.priority = params.priority
  if (params.assigneeId) query.assigneeId = params.assigneeId
  if (params.teamId) query.teamId = params.teamId
  if (params.dueBefore) query.dueBefore = params.dueBefore
  if (params.dueAfter) query.dueAfter = params.dueAfter
  return query
}
