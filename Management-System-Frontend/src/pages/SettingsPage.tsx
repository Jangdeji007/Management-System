import { useEffect, useState } from 'react'
import { getErrorMessage } from '../api/errors'
import { getCurrentUser } from '../api/authApi'
import type { UserProfile } from '../api/types'
import { ErrorAlert } from '../components/ErrorAlert'
import { LoadingSpinner } from '../components/LoadingSpinner'

export function SettingsPage() {
  const [profile, setProfile] = useState<UserProfile | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    let cancelled = false
    ;(async () => {
      try {
        const data = await getCurrentUser()
        if (!cancelled) setProfile(data)
      } catch (err) {
        if (!cancelled) setError(getErrorMessage(err))
      } finally {
        if (!cancelled) setLoading(false)
      }
    })()
    return () => {
      cancelled = true
    }
  }, [])

  if (loading) return <LoadingSpinner />

  return (
    <div className="mx-auto max-w-lg space-y-6">
      <div>
        <h1 className="page-heading">Settings</h1>
        <p className="text-slate-600">Your profile (read-only)</p>
      </div>

      <ErrorAlert message={error} />

      {profile && (
        <dl className="card-surface space-y-4 text-sm">
          <div>
            <dt className="font-medium text-slate-500">Full name</dt>
            <dd className="mt-1 text-slate-900">{profile.fullName}</dd>
          </div>
          <div>
            <dt className="font-medium text-slate-500">Email</dt>
            <dd className="mt-1 text-slate-900">{profile.email}</dd>
          </div>
          <div>
            <dt className="font-medium text-slate-500">Role</dt>
            <dd className="mt-1 text-slate-900">{profile.role}</dd>
          </div>
          <div>
            <dt className="font-medium text-slate-500">Member since</dt>
            <dd className="mt-1 text-slate-900">
              {new Date(profile.createdAt).toLocaleDateString()}
            </dd>
          </div>
        </dl>
      )}

      <p className="text-xs text-slate-500">
        Profile editing is not available yet. Contact an administrator to change your role.
      </p>
    </div>
  )
}
