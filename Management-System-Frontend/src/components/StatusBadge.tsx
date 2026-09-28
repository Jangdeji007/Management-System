import type { TaskStatusName } from '../api/enums'

const styles: Record<TaskStatusName, string> = {
  ToDo: 'bg-slate-100 text-slate-700',
  InProgress: 'bg-amber-100 text-amber-900',
  Done: 'bg-emerald-100 text-emerald-900',
}

const labels: Record<TaskStatusName, string> = {
  ToDo: 'To Do',
  InProgress: 'In Progress',
  Done: 'Done',
}

export function StatusBadge({ status }: { status: TaskStatusName }) {
  return (
    <span className={`inline-flex rounded-full px-2.5 py-0.5 text-xs font-medium ${styles[status]}`}>
      {labels[status]}
    </span>
  )
}
