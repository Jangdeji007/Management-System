import type { TaskPriorityName, TaskStatusName, UserRoleName } from './enums'

export interface UserProfile {
  id: string
  email: string
  fullName: string
  role: UserRoleName
  createdAt: string
}

export interface AuthResponse {
  accessToken: string
  user: UserProfile
}

export interface TaskListItem {
  id: string
  title: string
  status: TaskStatusName
  priority: TaskPriorityName
  dueDate: string | null
  assigneeId: string
  assigneeName: string
  teamId: string | null
  teamName: string | null
  createdAt: string
  updatedAt: string
}

export interface TaskComment {
  id: string
  taskId: string
  authorId: string
  authorName: string
  body: string
  createdAt: string
}

export interface TaskDetail extends Omit<TaskListItem, never> {
  description: string | null
  createdById: string
  createdByName: string
  comments: TaskComment[]
}

export interface TaskStatusCounts {
  toDo: number
  inProgress: number
  done: number
  total: number
}

export interface DashboardSummary {
  taskCounts: TaskStatusCounts
  unreadNotificationCount: number
}

export interface UserSummary {
  id: string
  email: string
  fullName: string
  role: UserRoleName
}

export interface TeamMember {
  userId: string
  email: string
  fullName: string
  role: UserRoleName
  joinedAt: string
}

export interface TeamListItem {
  id: string
  name: string
  description: string | null
  createdByUserId: string
  createdAt: string
  members: TeamMember[]
}

export interface NotificationItem {
  id: string
  type: string
  message: string
  relatedTaskId: string | null
  isRead: boolean
  createdAt: string
}

export interface TaskQueryParams {
  status?: TaskStatusName
  priority?: TaskPriorityName
  assigneeId?: string
  teamId?: string
  dueBefore?: string
  dueAfter?: string
}

export interface ApiErrorBody {
  title?: string
  status?: number
  errors?: Record<string, string[]>
}
