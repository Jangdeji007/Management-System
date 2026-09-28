import { useEffect, useState, type FormEvent } from 'react'
import { Link, useParams } from 'react-router-dom'
import { toast } from 'sonner'
import { getErrorMessage } from '../api/errors'
import { listTasks } from '../api/tasksApi'
import {
  addTeamMember,
  listTeams,
  removeTeamMember,
  updateTeam,
} from '../api/teamsApi'
import { listUsers } from '../api/usersApi'
import type { TaskListItem, TeamListItem, UserSummary } from '../api/types'
import { useAuth } from '../auth/AuthContext'
import { isAdmin, isManagerOrAdmin } from '../auth/roles'
import { ErrorAlert } from '../components/ErrorAlert'
import { LoadingSpinner } from '../components/LoadingSpinner'
import { PriorityBadge } from '../components/PriorityBadge'
import { StatusBadge } from '../components/StatusBadge'
import { linkPrimary } from '../styles/buttons'

function formatDate(value: string | null): string {
  if (!value) return '—'
  return new Date(value).toLocaleDateString()
}

function memberInitials(name: string): string {
  return name
    .split(/\s+/)
    .map((p) => p[0])
    .join('')
    .slice(0, 2)
    .toUpperCase()
}

export function TeamDetailPage() {
  const { id } = useParams<{ id: string }>()
  const { user } = useAuth()
  const [team, setTeam] = useState<TeamListItem | null>(null)
  const [teamTasks, setTeamTasks] = useState<TaskListItem[]>([])
  const [allUsers, setAllUsers] = useState<UserSummary[]>([])
  const [memberPick, setMemberPick] = useState('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const load = async (teamId: string) => {
    const [teams, tasks, users] = await Promise.all([
      listTeams(),
      listTasks({ teamId }),
      listUsers(),
    ])
    const found = teams.find((t) => t.id === teamId) ?? null
    setTeam(found)
    setTeamTasks(tasks)
    setAllUsers(users)
  }

  useEffect(() => {
    if (!id || !user || !isManagerOrAdmin(user.role)) return
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
  }, [id, user])

  if (!user || !isManagerOrAdmin(user.role)) {
    return <p className="text-slate-600">You do not have access to this team.</p>
  }

  const onAddMember = async () => {
    if (!id || !memberPick) return
    setError('')
    try {
      await addTeamMember(id, memberPick)
      toast.success('Member added')
      setMemberPick('')
      await load(id)
    } catch (err) {
      setError(getErrorMessage(err))
    }
  }

  const onRemoveMember = async (userId: string) => {
    if (!id) return
    setError('')
    try {
      await removeTeamMember(id, userId)
      toast.success('Member removed')
      await load(id)
    } catch (err) {
      setError(getErrorMessage(err))
    }
  }

  const onRename = async (e: FormEvent) => {
    e.preventDefault()
    if (!team || !user || !isAdmin(user.role)) return
    const name = (e.target as HTMLFormElement).teamName.value as string
    if (!name.trim()) return
    setError('')
    try {
      await updateTeam(team.id, name.trim(), team.description ?? undefined)
      toast.success('Team updated')
      await load(team.id)
    } catch (err) {
      setError(getErrorMessage(err))
    }
  }

  if (loading) return <LoadingSpinner />

  if (!team) {
    return (
      <div className="space-y-4">
        <ErrorAlert message={error || 'Team not found.'} />
        <Link to="/teams" className={linkPrimary}>
          ← Back to teams
        </Link>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <Link to="/teams" className={`text-sm ${linkPrimary}`}>
        ← Back to teams
      </Link>

      <div>
        <h1 className="page-heading">{team.name}</h1>
        {team.description && <p className="text-slate-600">{team.description}</p>}
        <p className="mt-1 text-sm text-slate-500">{team.members.length} members</p>
      </div>

      <ErrorAlert message={error} />

      {user && isAdmin(user.role) && (
        <form onSubmit={onRename} className="flex flex-col gap-2 sm:flex-row sm:items-end">
          <label className="flex-1 text-sm">
            <span className="font-medium text-slate-700">Team name</span>
            <input
              name="teamName"
              defaultValue={team.name}
              className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2"
            />
          </label>
          <button
            type="submit"
            className="rounded-lg border border-slate-300 px-4 py-2.5 text-sm font-medium hover:bg-slate-50"
          >
            Save name
          </button>
        </form>
      )}

      <section className="card-surface">
        <h2 className="text-lg font-semibold text-slate-900">Members</h2>
        <ul className="mt-3 space-y-2">
          {team.members.map((m) => (
            <li key={m.userId} className="flex items-center justify-between gap-2 text-sm">
              <span className="flex items-center gap-2">
                <span className="flex h-8 w-8 items-center justify-center rounded-full bg-blue-100 text-xs font-semibold text-blue-800">
                  {memberInitials(m.fullName)}
                </span>
                {m.fullName} <span className="text-slate-500">({m.role})</span>
              </span>
              <button
                type="button"
                className="text-xs text-red-600 hover:underline"
                onClick={() => void onRemoveMember(m.userId)}
              >
                Remove
              </button>
            </li>
          ))}
        </ul>

        <div className="mt-4 flex flex-col gap-2 sm:flex-row">
          <select
            className="min-w-0 flex-1 rounded-lg border border-slate-300 px-3 py-2 text-sm"
            value={memberPick}
            onChange={(e) => setMemberPick(e.target.value)}
          >
            <option value="">Add member…</option>
            {allUsers
              .filter((u) => !team.members.some((m) => m.userId === u.id))
              .map((u) => (
                <option key={u.id} value={u.id}>
                  {u.fullName} ({u.email})
                </option>
              ))}
          </select>
          <button
            type="button"
            className="rounded-lg border border-slate-300 px-4 py-2.5 text-sm font-medium hover:bg-slate-50"
            onClick={() => void onAddMember()}
          >
            Add
          </button>
        </div>
      </section>

      <section className="card-surface">
        <h2 className="text-lg font-semibold text-slate-900">Team tasks</h2>
        {teamTasks.length === 0 ? (
          <p className="mt-2 text-sm text-slate-500">No tasks linked to this team.</p>
        ) : (
          <ul className="mt-3 divide-y divide-slate-100">
            {teamTasks.map((task) => (
              <li key={task.id} className="flex flex-wrap items-center justify-between gap-2 py-3">
                <Link to={`/tasks/${task.id}`} className={linkPrimary}>
                  {task.title}
                </Link>
                <div className="flex items-center gap-2">
                  <StatusBadge status={task.status} />
                  <PriorityBadge priority={task.priority} />
                  <span className="text-xs text-slate-500">{formatDate(task.dueDate)}</span>
                </div>
              </li>
            ))}
          </ul>
        )}
      </section>
    </div>
  )
}
