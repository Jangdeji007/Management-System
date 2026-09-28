import { apiClient } from './client'
import type { TaskPriorityName, TaskStatusName } from './enums'
import { TaskPriorityApiValue, TaskStatusApiValue } from './enums'
import { buildTaskQueryParams } from './query'
import type { TaskDetail, TaskListItem, TaskQueryParams } from './types'

export async function listTasks(params: TaskQueryParams = {}): Promise<TaskListItem[]> {
  const { data } = await apiClient.get<TaskListItem[]>('/api/tasks', {
    params: buildTaskQueryParams(params),
  })
  return data
}

export async function getTask(id: string): Promise<TaskDetail> {
  const { data } = await apiClient.get<TaskDetail>(`/api/tasks/${id}`)
  return data
}

export interface CreateTaskPayload {
  title: string
  description?: string
  priority?: TaskPriorityName
  dueDate?: string
  assigneeId: string
  teamId?: string
}

export async function createTask(payload: CreateTaskPayload): Promise<TaskDetail> {
  const { data } = await apiClient.post<TaskDetail>('/api/tasks', {
    title: payload.title,
    description: payload.description ?? null,
    priority: payload.priority ? TaskPriorityApiValue[payload.priority] : 1,
    dueDate: payload.dueDate ?? null,
    assigneeId: payload.assigneeId,
    teamId: payload.teamId ?? null,
  })
  return data
}

export async function updateTaskStatus(
  id: string,
  status: TaskStatusName,
): Promise<TaskDetail> {
  const { data } = await apiClient.patch<TaskDetail>(`/api/tasks/${id}/status`, {
    status: TaskStatusApiValue[status],
  })
  return data
}

export async function addTaskComment(taskId: string, body: string): Promise<TaskDetail> {
  await apiClient.post(`/api/tasks/${taskId}/comments`, { body })
  return getTask(taskId)
}
