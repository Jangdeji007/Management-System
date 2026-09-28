export const TaskStatus = {
  ToDo: 'ToDo',
  InProgress: 'InProgress',
  Done: 'Done',
} as const

export type TaskStatusName = (typeof TaskStatus)[keyof typeof TaskStatus]

export const TaskStatusApiValue: Record<TaskStatusName, number> = {
  ToDo: 0,
  InProgress: 1,
  Done: 2,
}

export const TaskPriority = {
  Low: 'Low',
  Medium: 'Medium',
  High: 'High',
} as const

export type TaskPriorityName = (typeof TaskPriority)[keyof typeof TaskPriority]

export const TaskPriorityApiValue: Record<TaskPriorityName, number> = {
  Low: 0,
  Medium: 1,
  High: 2,
}

export const UserRole = {
  Admin: 'Admin',
  Manager: 'Manager',
  User: 'User',
} as const

export type UserRoleName = (typeof UserRole)[keyof typeof UserRole]
