import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getDashboardSummary } from '../api/dashboardApi'
import { getErrorMessage } from '../api/errors'
import { listTasks } from '../api/tasksApi'
import type { DashboardSummary, TaskListItem } from '../api/types'
import { useAuth } from '../auth/AuthContext'
import { ErrorAlert } from '../components/ErrorAlert'
import { LoadingSpinner } from '../components/LoadingSpinner'
import { PriorityBadge } from '../components/PriorityBadge'
import { StatusBadge } from '../components/StatusBadge'
import { TaskFilters } from '../components/TaskFilters'
import { TaskStatusChart } from '../components/TaskStatusChart'
import { useTaskFiltersFromSearch } from '../hooks/useTaskFiltersFromSearch'
import { linkPrimary } from '../styles/buttons'
import { recentTasks, upcomingTasks } from '../utils/taskListHelpers'

function StatCard({ label, value }: { label: string; value: number }) {
  return (
    <div className="rounded-xl border border-slate-200 bg-white p-4 sm:p-5">
      <p className="text-xs font-medium text-slate-600 sm:text-sm">{label}</p>
      <p className="mt-1 text-2xl font-bold text-slate-900 sm:mt-2 sm:text-3xl">{value}</p>
    </div>
  )
}

function formatDate(value: string | null): string {
  if (!value) return '—'
  return new Date(value).toLocaleDateString()
}

export function DashboardPage() {
  const { user } = useAuth()
  const { filters, setFilters } = useTaskFiltersFromSearch()
  const [summary, setSummary] = useState<DashboardSummary | null>(null)
  const [allTasks, setAllTasks] = useState<TaskListItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    let cancelled = false
    ;(async () => {
      setLoading(true)
      setError('')
      try {
        const [s, tAll] = await Promise.all([getDashboardSummary(filters), listTasks({})])
        if (!cancelled) {
          setSummary(s)
          setAllTasks(tAll)
        }
      } catch (err) {
        if (!cancelled) setError(getErrorMessage(err))
      } finally {
        if (!cancelled) setLoading(false)
      }
    })()
    return () => {
      cancelled = true
    }
  }, [filters])

  const recent = recentTasks(allTasks, 8)
  const upcoming = upcomingTasks(allTasks, 5)

  return (
    <div className="space-y-6">
      <div>
        <h1 className="page-heading">Dashboard</h1>
        <p className="text-sm text-slate-600 sm:text-base">
          {user ? `Welcome back, ${user.fullName}` : 'Overview of tasks in your scope'}
        </p>
      </div>

      <TaskFilters filters={filters} onChange={setFilters} />
      <ErrorAlert message={error} />

      {loading ? (
        <LoadingSpinner />
      ) : summary ? (
        <>
          <div className="grid grid-cols-2 gap-3 sm:gap-4 lg:grid-cols-4">
            <StatCard label="Total tasks" value={summary.taskCounts.total} />
            <StatCard label="To Do" value={summary.taskCounts.toDo} />
            <StatCard label="In Progress" value={summary.taskCounts.inProgress} />
            <StatCard label="Completed" value={summary.taskCounts.done} />
          </div>

          <div className="grid gap-6 lg:grid-cols-2">
            <TaskStatusChart counts={summary.taskCounts} />

            <section className="card-surface">
              <h2 className="text-lg font-semibold text-slate-900">Upcoming deadlines</h2>
              {upcoming.length === 0 ? (
                <p className="mt-3 text-sm text-slate-500">No upcoming due dates.</p>
              ) : (
                <ul className="mt-3 divide-y divide-slate-100">
                  {upcoming.map((task) => (
                    <li key={task.id} className="flex flex-wrap items-center justify-between gap-2 py-3">
                      <Link to={`/tasks/${task.id}`} className={linkPrimary}>
                        {task.title}
                      </Link>
                      <span className="text-sm text-slate-600">{formatDate(task.dueDate)}</span>
                    </li>
                  ))}
                </ul>
              )}
            </section>
          </div>

          <section className="card-surface">
            <h2 className="text-lg font-semibold text-slate-900">Recent tasks</h2>
            {recent.length === 0 ? (
              <p className="mt-3 text-sm text-slate-500">No tasks yet.</p>
            ) : (
              <ul className="mt-3 divide-y divide-slate-100">
                {recent.map((task) => (
                  <li
                    key={task.id}
                    className="flex flex-col gap-2 py-3 sm:flex-row sm:flex-wrap sm:items-center sm:justify-between"
                  >
                    <Link to={`/tasks/${task.id}`} className={linkPrimary}>
                      {task.title}
                    </Link>
                    <div className="flex flex-wrap items-center gap-2">
                      <StatusBadge status={task.status} />
                      <PriorityBadge priority={task.priority} />
                    </div>
                  </li>
                ))}
              </ul>
            )}
          </section>
        </>
      ) : null}
    </div>
  )
}
