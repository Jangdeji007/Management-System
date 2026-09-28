import { useEffect, useMemo, useState } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'
import { Plus, Search } from 'lucide-react'
import { getErrorMessage } from '../api/errors'
import { listTasks } from '../api/tasksApi'
import type { TaskListItem } from '../api/types'
import { useAuth } from '../auth/AuthContext'
import { isManagerOrAdmin } from '../auth/roles'
import { CreateTaskModal } from '../components/CreateTaskModal'
import { EmptyState } from '../components/EmptyState'
import { ErrorAlert } from '../components/ErrorAlert'
import { LoadingSpinner } from '../components/LoadingSpinner'
import { PriorityBadge } from '../components/PriorityBadge'
import { StatusBadge } from '../components/StatusBadge'
import { TaskFilters } from '../components/TaskFilters'
import { useTaskFiltersFromSearch } from '../hooks/useTaskFiltersFromSearch'
import { btnPrimary, linkPrimary } from '../styles/buttons'
import { filterTasksBySearch } from '../utils/taskListHelpers'

function formatDate(value: string | null): string {
  if (!value) return '—'
  return new Date(value).toLocaleDateString()
}

export function TasksPage() {
  const { user } = useAuth()
  const navigate = useNavigate()
  const [searchParams, setSearchParams] = useSearchParams()
  const { filters, setFilters } = useTaskFiltersFromSearch()
  const [tasks, setTasks] = useState<TaskListItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [search, setSearch] = useState('')
  const [createOpen, setCreateOpen] = useState(searchParams.get('create') === '1')

  const canCreate = user && isManagerOrAdmin(user.role)

  useEffect(() => {
    if (searchParams.get('create') === '1') setCreateOpen(true)
  }, [searchParams])

  const closeCreate = () => {
    setCreateOpen(false)
    if (searchParams.get('create')) {
      searchParams.delete('create')
      setSearchParams(searchParams, { replace: true })
    }
  }

  useEffect(() => {
    let cancelled = false
    ;(async () => {
      setLoading(true)
      setError('')
      try {
        const data = await listTasks(filters)
        if (!cancelled) setTasks(data)
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

  const visibleTasks = useMemo(() => filterTasksBySearch(tasks, search), [tasks, search])

  const reload = async () => {
    const data = await listTasks(filters)
    setTasks(data)
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:flex-wrap sm:items-center sm:justify-between sm:gap-4">
        <div>
          <h1 className="page-heading">Tasks</h1>
          <p className="text-sm text-slate-600 sm:text-base">View and manage tasks</p>
        </div>
        {canCreate && (
          <button type="button" className={`inline-flex w-full items-center justify-center gap-2 sm:w-auto ${btnPrimary}`} onClick={() => setCreateOpen(true)}>
            <Plus className="h-4 w-4" />
            Create task
          </button>
        )}
      </div>

      <label className="relative block">
        <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400" />
        <input
          type="search"
          placeholder="Search tasks…"
          className="w-full rounded-xl border border-slate-200 bg-white py-2.5 pl-10 pr-3 text-sm"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </label>

      <TaskFilters filters={filters} onChange={setFilters} />
      <ErrorAlert message={error} />

      {loading ? (
        <LoadingSpinner />
      ) : visibleTasks.length === 0 ? (
        <EmptyState
          title="No tasks found"
          description={
            search || Object.keys(filters).length > 0
              ? 'There are no tasks matching your filters.'
              : 'Create a task to get started.'
          }
          action={
            Object.keys(filters).length > 0 || search ? (
              <button
                type="button"
                className="text-sm font-medium text-blue-600 hover:text-blue-800"
                onClick={() => {
                  setSearch('')
                  setFilters({})
                }}
              >
                Clear filters
              </button>
            ) : canCreate ? (
              <button type="button" className={btnPrimary} onClick={() => setCreateOpen(true)}>
                Create task
              </button>
            ) : undefined
          }
        />
      ) : (
        <>
          <ul className="space-y-3 md:hidden">
            {visibleTasks.map((task) => (
              <li key={task.id} className="card-surface space-y-2">
                <Link to={`/tasks/${task.id}`} className={`block ${linkPrimary}`}>
                  {task.title}
                </Link>
                <div className="flex flex-wrap items-center gap-2">
                  <StatusBadge status={task.status} />
                  <PriorityBadge priority={task.priority} />
                </div>
                <dl className="grid grid-cols-2 gap-2 text-xs text-slate-600">
                  <div>
                    <dt className="text-slate-500">Assignee</dt>
                    <dd className="font-medium text-slate-800">{task.assigneeName}</dd>
                  </div>
                  <div>
                    <dt className="text-slate-500">Due</dt>
                    <dd className="font-medium text-slate-800">{formatDate(task.dueDate)}</dd>
                  </div>
                </dl>
              </li>
            ))}
          </ul>

          <div className="hidden overflow-x-auto rounded-xl border border-slate-200 bg-white md:block">
            <table className="min-w-full text-left text-sm">
              <thead className="border-b border-slate-200 bg-slate-50 text-slate-600">
                <tr>
                  <th className="px-4 py-3 font-medium">Task</th>
                  <th className="px-4 py-3 font-medium">Assigned to</th>
                  <th className="px-4 py-3 font-medium">Priority</th>
                  <th className="px-4 py-3 font-medium">Status</th>
                  <th className="px-4 py-3 font-medium">Due date</th>
                  <th className="px-4 py-3 font-medium">Action</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {visibleTasks.map((task) => (
                  <tr key={task.id} className="hover:bg-slate-50">
                    <td className="px-4 py-3 font-medium text-slate-900">{task.title}</td>
                    <td className="px-4 py-3 text-slate-700">{task.assigneeName}</td>
                    <td className="px-4 py-3">
                      <PriorityBadge priority={task.priority} />
                    </td>
                    <td className="px-4 py-3">
                      <StatusBadge status={task.status} />
                    </td>
                    <td className="px-4 py-3 text-slate-600">{formatDate(task.dueDate)}</td>
                    <td className="px-4 py-3">
                      <Link to={`/tasks/${task.id}`} className="text-sm font-medium text-blue-600 hover:underline">
                        View
                      </Link>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </>
      )}

      {canCreate && (
        <CreateTaskModal
          open={createOpen}
          onClose={closeCreate}
          onCreated={(id) => {
            void reload()
            navigate(`/tasks/${id}`)
          }}
        />
      )}
    </div>
  )
}
