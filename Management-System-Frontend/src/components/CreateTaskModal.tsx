import { useEffect, useState, type FormEvent } from 'react'
import { X } from 'lucide-react'
import { toast } from 'sonner'
import { getErrorMessage } from '../api/errors'
import { TaskPriority, type TaskPriorityName } from '../api/enums'
import { createTask } from '../api/tasksApi'
import { listTeams } from '../api/teamsApi'
import { listUsers } from '../api/usersApi'
import type { TeamListItem, UserSummary } from '../api/types'
import { ErrorAlert } from './ErrorAlert'
import { LoadingSpinner } from './LoadingSpinner'
import { btnPrimary } from '../styles/buttons'

interface CreateTaskModalProps {
  open: boolean
  onClose: () => void
  onCreated?: (taskId: string) => void
}

export function CreateTaskModal({ open, onClose, onCreated }: CreateTaskModalProps) {
  const [teams, setTeams] = useState<TeamListItem[]>([])
  const [users, setUsers] = useState<UserSummary[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [submitting, setSubmitting] = useState(false)

  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [priority, setPriority] = useState<TaskPriorityName>(TaskPriority.Medium)
  const [dueDate, setDueDate] = useState('')
  const [teamId, setTeamId] = useState('')
  const [assigneeId, setAssigneeId] = useState('')

  useEffect(() => {
    if (!open) return
    let cancelled = false
    ;(async () => {
      setLoading(true)
      setError('')
      try {
        const teamList = await listTeams()
        if (cancelled) return
        setTeams(teamList)
        const userList = await listUsers(teamList[0]?.id)
        if (cancelled) return
        setUsers(userList)
        setTeamId(teamList[0]?.id ?? '')
        setAssigneeId(userList[0]?.id ?? '')
      } catch (err) {
        if (!cancelled) setError(getErrorMessage(err))
      } finally {
        if (!cancelled) setLoading(false)
      }
    })()
    return () => {
      cancelled = true
    }
  }, [open])

  useEffect(() => {
    if (!open) return
    const onKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape') onClose()
    }
    document.addEventListener('keydown', onKey)
    return () => document.removeEventListener('keydown', onKey)
  }, [open, onClose])

  const onTeamChange = async (nextTeamId: string) => {
    setTeamId(nextTeamId)
    setAssigneeId('')
    try {
      const userList = await listUsers(nextTeamId || undefined)
      setUsers(userList)
      if (userList[0]) setAssigneeId(userList[0].id)
    } catch (err) {
      setError(getErrorMessage(err))
    }
  }

  const resetForm = () => {
    setTitle('')
    setDescription('')
    setPriority(TaskPriority.Medium)
    setDueDate('')
    setError('')
  }

  const handleClose = () => {
    resetForm()
    onClose()
  }

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setSubmitting(true)
    setError('')
    try {
      const created = await createTask({
        title,
        description: description || undefined,
        priority,
        dueDate: dueDate ? `${dueDate}T12:00:00` : undefined,
        assigneeId,
        teamId: teamId || undefined,
      })
      toast.success('Task created successfully')
      resetForm()
      onClose()
      onCreated?.(created.id)
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setSubmitting(false)
    }
  }

  if (!open) return null

  return (
    <div className="fixed inset-0 z-50 flex items-end justify-center p-0 sm:items-center sm:p-4">
      <button
        type="button"
        className="absolute inset-0 bg-slate-900/40"
        aria-label="Close dialog"
        onClick={handleClose}
      />
      <div
        role="dialog"
        aria-modal="true"
        aria-labelledby="create-task-title"
        className="relative max-h-[90vh] w-full max-w-lg overflow-y-auto rounded-t-2xl border border-slate-200 bg-white p-6 shadow-xl sm:rounded-2xl"
      >
        <div className="mb-4 flex items-center justify-between gap-2">
          <h2 id="create-task-title" className="text-lg font-semibold text-slate-900">
            Create new task
          </h2>
          <button
            type="button"
            className="rounded-lg p-2 text-slate-500 hover:bg-slate-100"
            aria-label="Close"
            onClick={handleClose}
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        <ErrorAlert message={error} />

        {loading ? (
          <LoadingSpinner />
        ) : (
          <form className="space-y-4" onSubmit={onSubmit}>
            <label className="block text-sm">
              <span className="font-medium text-slate-700">Task title</span>
              <input
                required
                maxLength={300}
                className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
              />
            </label>

            <label className="block text-sm">
              <span className="font-medium text-slate-700">Description</span>
              <textarea
                rows={3}
                maxLength={4000}
                className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
              />
            </label>

            <label className="block text-sm">
              <span className="font-medium text-slate-700">Assign to</span>
              <select
                required
                className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2"
                value={assigneeId}
                onChange={(e) => setAssigneeId(e.target.value)}
              >
                {users.length === 0 ? (
                  <option value="">No users available</option>
                ) : (
                  users.map((u) => (
                    <option key={u.id} value={u.id}>
                      {u.fullName} ({u.role})
                    </option>
                  ))
                )}
              </select>
            </label>

            <label className="block text-sm">
              <span className="font-medium text-slate-700">Priority</span>
              <select
                className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2"
                value={priority}
                onChange={(e) => setPriority(e.target.value as TaskPriorityName)}
              >
                {Object.values(TaskPriority).map((p) => (
                  <option key={p} value={p}>
                    {p}
                  </option>
                ))}
              </select>
            </label>

            <label className="block text-sm">
              <span className="font-medium text-slate-700">Due date</span>
              <input
                type="date"
                className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2"
                value={dueDate}
                onChange={(e) => setDueDate(e.target.value)}
              />
            </label>

            <label className="block text-sm">
              <span className="font-medium text-slate-700">Team (optional)</span>
              <select
                className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2"
                value={teamId}
                onChange={(e) => void onTeamChange(e.target.value)}
              >
                <option value="">No team</option>
                {teams.map((t) => (
                  <option key={t.id} value={t.id}>
                    {t.name}
                  </option>
                ))}
              </select>
            </label>

            <div className="flex flex-col-reverse gap-2 pt-2 sm:flex-row sm:justify-end">
              <button
                type="button"
                className="rounded-lg border border-slate-300 px-4 py-2.5 text-sm font-medium text-slate-700 hover:bg-slate-50"
                onClick={handleClose}
              >
                Cancel
              </button>
              <button type="submit" disabled={submitting || !assigneeId} className={btnPrimary}>
                {submitting ? 'Creating…' : 'Create task'}
              </button>
            </div>
          </form>
        )}
      </div>
    </div>
  )
}
