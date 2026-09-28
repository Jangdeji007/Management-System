import { TaskPriority, TaskStatus, type TaskPriorityName, type TaskStatusName } from '../api/enums'
import type { TaskQueryParams } from '../api/types'

interface TaskFiltersProps {
  filters: TaskQueryParams
  onChange: (next: TaskQueryParams) => void
}

export function TaskFilters({ filters, onChange }: TaskFiltersProps) {
  return (
    <div className="grid min-w-0 gap-3 rounded-xl border border-slate-200 bg-white p-3 sm:p-4 sm:grid-cols-2 lg:grid-cols-4">
      <label className="flex min-w-0 flex-col gap-1 text-sm">
        <span className="font-medium text-slate-700">Status</span>
        <select
          className="min-w-0 w-full rounded-lg border border-slate-300 px-3 py-2"
          value={filters.status ?? ''}
          onChange={(e) =>
            onChange({
              ...filters,
              status: (e.target.value || undefined) as TaskStatusName | undefined,
            })
          }
        >
          <option value="">All</option>
          {Object.values(TaskStatus).map((s) => (
            <option key={s} value={s}>
              {s === 'InProgress' ? 'In Progress' : s === 'ToDo' ? 'To Do' : s}
            </option>
          ))}
        </select>
      </label>

      <label className="flex min-w-0 flex-col gap-1 text-sm">
        <span className="font-medium text-slate-700">Priority</span>
        <select
          className="min-w-0 w-full rounded-lg border border-slate-300 px-3 py-2"
          value={filters.priority ?? ''}
          onChange={(e) =>
            onChange({
              ...filters,
              priority: (e.target.value || undefined) as TaskPriorityName | undefined,
            })
          }
        >
          <option value="">All</option>
          {Object.values(TaskPriority).map((p) => (
            <option key={p} value={p}>
              {p}
            </option>
          ))}
        </select>
      </label>

      <label className="flex min-w-0 flex-col gap-1 text-sm">
        <span className="font-medium text-slate-700">Due after</span>
        <input
          type="date"
          className="min-w-0 w-full rounded-lg border border-slate-300 px-3 py-2"
          value={filters.dueAfter?.slice(0, 10) ?? ''}
          onChange={(e) =>
            onChange({
              ...filters,
              dueAfter: e.target.value ? `${e.target.value}T00:00:00` : undefined,
            })
          }
        />
      </label>

      <label className="flex min-w-0 flex-col gap-1 text-sm">
        <span className="font-medium text-slate-700">Due before</span>
        <input
          type="date"
          className="min-w-0 w-full rounded-lg border border-slate-300 px-3 py-2"
          value={filters.dueBefore?.slice(0, 10) ?? ''}
          onChange={(e) =>
            onChange({
              ...filters,
              dueBefore: e.target.value ? `${e.target.value}T23:59:59` : undefined,
            })
          }
        />
      </label>

      <div className="sm:col-span-2 lg:col-span-4">
        <button
          type="button"
          className="text-sm font-medium text-blue-600 hover:text-blue-800"
          onClick={() => onChange({})}
        >
          Clear filters
        </button>
      </div>
    </div>
  )
}
