import type { TaskStatusCounts } from '../api/types'

const segments: { key: keyof TaskStatusCounts; label: string; bar: string }[] = [
  { key: 'toDo', label: 'To Do', bar: 'bg-slate-400' },
  { key: 'inProgress', label: 'In Progress', bar: 'bg-amber-500' },
  { key: 'done', label: 'Done', bar: 'bg-emerald-500' },
]

export function TaskStatusChart({ counts }: { counts: TaskStatusCounts }) {
  const total = counts.total || 1

  return (
    <div className="card-surface space-y-4">
      <h2 className="text-lg font-semibold text-slate-900">Task overview</h2>
      <div className="flex h-3 overflow-hidden rounded-full bg-slate-100">
        {segments.map(({ key, bar }) => {
          const value = counts[key]
          if (value === 0) return null
          const pct = (value / total) * 100
          return (
            <div
              key={key}
              className={`${bar} transition-all`}
              style={{ width: `${pct}%` }}
              title={`${key}: ${value}`}
            />
          )
        })}
      </div>
      <ul className="grid gap-2 sm:grid-cols-3">
        {segments.map(({ key, label, bar }) => (
          <li key={key} className="flex items-center justify-between gap-2 text-sm">
            <span className="flex items-center gap-2 text-slate-600">
              <span className={`h-2.5 w-2.5 rounded-full ${bar}`} aria-hidden />
              {label}
            </span>
            <span className="font-semibold text-slate-900">{counts[key]}</span>
          </li>
        ))}
      </ul>
    </div>
  )
}
