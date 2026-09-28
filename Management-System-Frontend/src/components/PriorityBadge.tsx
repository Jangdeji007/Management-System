import type { TaskPriorityName } from '../api/enums'

const styles: Record<TaskPriorityName, string> = {
  Low: 'bg-sky-50 text-sky-800',
  Medium: 'bg-violet-50 text-violet-800',
  High: 'bg-rose-50 text-rose-800',
}

export function PriorityBadge({ priority }: { priority: TaskPriorityName }) {
  return (
    <span
      className={`inline-flex rounded-full px-2.5 py-0.5 text-xs font-medium ${styles[priority]}`}
    >
      {priority}
    </span>
  )
}
