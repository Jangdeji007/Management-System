import { useMemo } from 'react'
import { useSearchParams } from 'react-router-dom'
import { TaskPriority, TaskStatus, type TaskPriorityName, type TaskStatusName } from '../api/enums'
import type { TaskQueryParams } from '../api/types'

const statusValues = new Set<string>(Object.values(TaskStatus))
const priorityValues = new Set<string>(Object.values(TaskPriority))

function parseStatus(value: string | null): TaskStatusName | undefined {
  if (value && statusValues.has(value)) return value as TaskStatusName
  return undefined
}

function parsePriority(value: string | null): TaskPriorityName | undefined {
  if (value && priorityValues.has(value)) return value as TaskPriorityName
  return undefined
}

export function useTaskFiltersFromSearch(): {
  filters: TaskQueryParams
  setFilters: (next: TaskQueryParams) => void
} {
  const [searchParams, setSearchParams] = useSearchParams()

  const filters = useMemo<TaskQueryParams>(() => {
    return {
      status: parseStatus(searchParams.get('status')),
      priority: parsePriority(searchParams.get('priority')),
      dueAfter: searchParams.get('dueAfter') ?? undefined,
      dueBefore: searchParams.get('dueBefore') ?? undefined,
      teamId: searchParams.get('teamId') ?? undefined,
    }
  }, [searchParams])

  const setFilters = (next: TaskQueryParams) => {
    const params = new URLSearchParams()
    if (next.status) params.set('status', next.status)
    if (next.priority) params.set('priority', next.priority)
    if (next.dueAfter) params.set('dueAfter', next.dueAfter)
    if (next.dueBefore) params.set('dueBefore', next.dueBefore)
    if (next.teamId) params.set('teamId', next.teamId)
    setSearchParams(params, { replace: true })
  }

  return { filters, setFilters }
}
