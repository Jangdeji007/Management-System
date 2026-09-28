import { useCallback, useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getErrorMessage } from '../api/errors'
import { listNotifications, markNotificationRead } from '../api/notificationsApi'
import type { NotificationItem } from '../api/types'
import { ErrorAlert } from '../components/ErrorAlert'
import { EmptyState } from '../components/EmptyState'
import { LoadingSpinner } from '../components/LoadingSpinner'
import { linkPrimary } from '../styles/buttons'

export function NotificationsPage() {
  const [items, setItems] = useState<NotificationItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const load = useCallback(async () => {
    setError('')
    const data = await listNotifications()
    setItems(data)
  }, [])

  useEffect(() => {
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
  }, [load])

  const onMarkRead = async (id: string) => {
    await markNotificationRead(id)
    await load()
  }

  if (loading) return <LoadingSpinner />

  return (
    <div className="space-y-6">
      <div>
        <h1 className="page-heading">Notifications</h1>
        <p className="text-slate-600">Task assignments and status updates</p>
      </div>

      <ErrorAlert message={error} />

      {items.length === 0 ? (
        <EmptyState title="No notifications yet" />
      ) : (
        <ul className="divide-y divide-slate-100 overflow-hidden rounded-xl border border-slate-200 bg-white">
          {items.map((n) => (
            <li key={n.id} className={`px-4 py-4 sm:px-6 ${n.isRead ? 'opacity-75' : ''}`}>
              <p className="text-slate-800">{n.message}</p>
              <p className="mt-1 text-xs text-slate-500">{new Date(n.createdAt).toLocaleString()}</p>
              <div className="mt-2 flex flex-wrap gap-3">
                {n.relatedTaskId && (
                  <Link to={`/tasks/${n.relatedTaskId}`} className={`text-sm ${linkPrimary}`}>
                    View task
                  </Link>
                )}
                {!n.isRead && (
                  <button
                    type="button"
                    className="text-sm font-medium text-slate-600 hover:text-slate-900"
                    onClick={() => void onMarkRead(n.id)}
                  >
                    Mark read
                  </button>
                )}
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}
