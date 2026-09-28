import { useEffect, useState, type FormEvent } from 'react'
import { Link, useParams } from 'react-router-dom'
import { toast } from 'sonner'
import { getErrorMessage } from '../api/errors'
import { TaskStatus, type TaskStatusName } from '../api/enums'
import { addTaskComment, getTask, updateTaskStatus } from '../api/tasksApi'
import type { TaskDetail } from '../api/types'
import { ErrorAlert } from '../components/ErrorAlert'
import { LoadingSpinner } from '../components/LoadingSpinner'
import { PriorityBadge } from '../components/PriorityBadge'
import { StatusBadge } from '../components/StatusBadge'
import { btnPrimary, linkPrimary } from '../styles/buttons'

export function TaskDetailPage() {
  const { id } = useParams<{ id: string }>()
  const [task, setTask] = useState<TaskDetail | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [statusSaving, setStatusSaving] = useState(false)
  const [comment, setComment] = useState('')
  const [commentSaving, setCommentSaving] = useState(false)

  const load = async (taskId: string) => {
    setError('')
    const data = await getTask(taskId)
    setTask(data)
  }

  useEffect(() => {
    if (!id) return
    let cancelled = false
    ;(async () => {
      setLoading(true)
      try {
        await load(id)
      } catch (err) {
        if (!cancelled) setError(getErrorMessage(err))
      } finally {
        if (!cancelled) setLoading(false)
      }
    })()
    return () => {
      cancelled = true
    }
  }, [id])

  const onStatusChange = async (status: TaskStatusName) => {
    if (!id || !task) return
    setStatusSaving(true)
    setError('')
    try {
      const updated = await updateTaskStatus(id, status)
      setTask(updated)
      toast.success('Status updated')
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setStatusSaving(false)
    }
  }

  const onCommentSubmit = async (e: FormEvent) => {
    e.preventDefault()
    if (!id || !comment.trim()) return
    setCommentSaving(true)
    setError('')
    try {
      const updated = await addTaskComment(id, comment.trim())
      setTask(updated)
      setComment('')
      toast.success('Comment posted')
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setCommentSaving(false)
    }
  }

  if (loading) return <LoadingSpinner />
  if (!task) {
    return (
      <div className="space-y-4">
        <ErrorAlert message={error || 'Task not found.'} />
        <Link to="/tasks" className={linkPrimary}>
          Back to tasks
        </Link>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <Link to="/tasks" className={`text-sm ${linkPrimary}`}>
        ← Back to tasks
      </Link>

      <ErrorAlert message={error} />

      <article className="card-surface">
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div>
            <h1 className="page-heading">{task.title}</h1>
            <p className="mt-2 text-slate-600">{task.description || 'No description.'}</p>
          </div>
          <div className="flex flex-wrap gap-2">
            <StatusBadge status={task.status} />
            <PriorityBadge priority={task.priority} />
          </div>
        </div>

        <dl className="mt-6 grid gap-3 text-sm sm:grid-cols-2">
          <div>
            <dt className="text-slate-500">Assignee</dt>
            <dd className="font-medium text-slate-900">{task.assigneeName}</dd>
          </div>
          <div>
            <dt className="text-slate-500">Created by</dt>
            <dd className="font-medium text-slate-900">{task.createdByName}</dd>
          </div>
          <div>
            <dt className="text-slate-500">Team</dt>
            <dd className="font-medium text-slate-900">{task.teamName ?? '—'}</dd>
          </div>
          <div>
            <dt className="text-slate-500">Due date</dt>
            <dd className="font-medium text-slate-900">
              {task.dueDate ? new Date(task.dueDate).toLocaleDateString() : '—'}
            </dd>
          </div>
        </dl>

        <label className="mt-6 block w-full max-w-none text-sm sm:max-w-xs">
          <span className="font-medium text-slate-700">Update status</span>
          <select
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 disabled:opacity-60"
            value={task.status}
            disabled={statusSaving}
            onChange={(e) => void onStatusChange(e.target.value as TaskStatusName)}
          >
            {Object.values(TaskStatus).map((s) => (
              <option key={s} value={s}>
                {s === 'InProgress' ? 'In Progress' : s === 'ToDo' ? 'To Do' : s}
              </option>
            ))}
          </select>
        </label>
      </article>

      <section className="card-surface">
        <h2 className="text-lg font-semibold text-slate-900">Comments</h2>
        {task.comments.length === 0 ? (
          <p className="mt-4 text-sm text-slate-500">No comments yet.</p>
        ) : (
          <ul className="mt-4 space-y-4">
            {task.comments.map((c) => (
              <li key={c.id} className="rounded-lg bg-slate-50 px-4 py-3">
                <p className="text-sm text-slate-800">{c.body}</p>
                <p className="mt-1 text-xs text-slate-500">
                  {c.authorName} · {new Date(c.createdAt).toLocaleString()}
                </p>
              </li>
            ))}
          </ul>
        )}

        <form className="mt-6 space-y-3" onSubmit={onCommentSubmit}>
          <label className="block text-sm">
            <span className="font-medium text-slate-700">Add comment</span>
            <textarea
              required
              rows={3}
              className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2"
              value={comment}
              onChange={(e) => setComment(e.target.value)}
            />
          </label>
          <button
            type="submit"
            disabled={commentSaving}
            className={btnPrimary}
          >
            {commentSaving ? 'Posting…' : 'Post comment'}
          </button>
        </form>
      </section>
    </div>
  )
}
