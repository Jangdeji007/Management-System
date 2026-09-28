import type { TaskListItem } from '../api/types'

export function filterTasksBySearch(tasks: TaskListItem[], query: string): TaskListItem[] {
  const q = query.trim().toLowerCase()
  if (!q) return tasks
  return tasks.filter(
    (t) =>
      t.title.toLowerCase().includes(q) || t.assigneeName.toLowerCase().includes(q),
  )
}

export function recentTasks(tasks: TaskListItem[], limit = 8): TaskListItem[] {
  return [...tasks].sort((a, b) => b.updatedAt.localeCompare(a.updatedAt)).slice(0, limit)
}

export function upcomingTasks(tasks: TaskListItem[], limit = 5): TaskListItem[] {
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  return [...tasks]
    .filter((t) => t.dueDate && t.status !== 'Done')
    .filter((t) => new Date(t.dueDate!) >= today)
    .sort((a, b) => a.dueDate!.localeCompare(b.dueDate!))
    .slice(0, limit)
}
