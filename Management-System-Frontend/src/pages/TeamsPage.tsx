import { useEffect, useState, type FormEvent } from 'react'
import { Link } from 'react-router-dom'
import { toast } from 'sonner'
import { getErrorMessage } from '../api/errors'
import { createTeam, listTeams } from '../api/teamsApi'
import type { TeamListItem } from '../api/types'
import { useAuth } from '../auth/AuthContext'
import { isAdmin, isManagerOrAdmin } from '../auth/roles'
import { EmptyState } from '../components/EmptyState'
import { ErrorAlert } from '../components/ErrorAlert'
import { LoadingSpinner } from '../components/LoadingSpinner'
import { btnPrimary, linkPrimary } from '../styles/buttons'

function memberInitials(name: string): string {
  return name
    .split(/\s+/)
    .map((p) => p[0])
    .join('')
    .slice(0, 2)
    .toUpperCase()
}

export function TeamsPage() {
  const { user } = useAuth()
  const [teams, setTeams] = useState<TeamListItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [newName, setNewName] = useState('')
  const [newDescription, setNewDescription] = useState('')

  const load = async () => {
    setTeams(await listTeams())
  }

  useEffect(() => {
    if (!user || !isManagerOrAdmin(user.role)) return
    let cancelled = false
    ;(async () => {
      try {
        await load()
      } catch (err) {
        if (!cancelled) setError(getErrorMessage(err))
      } finally {
        if (!cancelled) setLoading(false)
      }
    })()
    return () => {
      cancelled = true
    }
  }, [user])

  if (!user || !isManagerOrAdmin(user.role)) {
    return <p className="text-slate-600">You do not have access to team management.</p>
  }

  const onCreateTeam = async (e: FormEvent) => {
    e.preventDefault()
    if (!isAdmin(user.role)) return
    setError('')
    try {
      await createTeam(newName, newDescription || undefined)
      toast.success('Team created')
      setNewName('')
      setNewDescription('')
      await load()
    } catch (err) {
      setError(getErrorMessage(err))
    }
  }

  if (loading) return <LoadingSpinner />

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="page-heading">{user.role === 'Manager' ? 'My Team' : 'Teams'}</h1>
          <p className="text-slate-600">Manage teams and membership</p>
        </div>
      </div>

      <ErrorAlert message={error} />

      {isAdmin(user.role) && (
        <form
          onSubmit={onCreateTeam}
          className="grid gap-3 rounded-xl border border-slate-200 bg-white p-4 sm:grid-cols-3"
        >
          <input
            required
            placeholder="Team name"
            className="rounded-lg border border-slate-300 px-3 py-2"
            value={newName}
            onChange={(e) => setNewName(e.target.value)}
          />
          <input
            placeholder="Description"
            className="rounded-lg border border-slate-300 px-3 py-2 sm:col-span-2"
            value={newDescription}
            onChange={(e) => setNewDescription(e.target.value)}
          />
          <button type="submit" className={`sm:col-span-3 sm:w-fit ${btnPrimary}`}>
            Create team
          </button>
        </form>
      )}

      {teams.length === 0 ? (
        <EmptyState
          title="No teams created yet"
          action={
            isAdmin(user.role) ? (
              <p className="text-sm text-slate-500">Use the form above to create your first team.</p>
            ) : undefined
          }
        />
      ) : (
        <ul className="grid gap-4 sm:grid-cols-2">
          {teams.map((team) => (
            <li key={team.id} className="card-surface flex flex-col">
              <h2 className="text-lg font-semibold text-slate-900">{team.name}</h2>
              {team.description && <p className="mt-1 text-sm text-slate-600">{team.description}</p>}
              <div className="mt-4 flex flex-wrap gap-1">
                {team.members.slice(0, 4).map((m) => (
                  <span
                    key={m.userId}
                    title={m.fullName}
                    className="flex h-8 w-8 items-center justify-center rounded-full bg-slate-100 text-xs font-medium text-slate-700"
                  >
                    {memberInitials(m.fullName)}
                  </span>
                ))}
                {team.members.length > 4 && (
                  <span className="flex h-8 items-center px-2 text-xs text-slate-500">
                    +{team.members.length - 4} more
                  </span>
                )}
              </div>
              <div className="mt-auto flex items-center justify-between pt-4">
                <span className="text-sm text-slate-500">{team.members.length} members</span>
                <Link to={`/teams/${team.id}`} className={`text-sm font-medium ${linkPrimary}`}>
                  View team →
                </Link>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}
