import { useCallback, useEffect, useRef, useState } from 'react'
import { Link } from 'react-router-dom'
import { Bell } from 'lucide-react'
import { listNotifications, markNotificationRead } from '../api/notificationsApi'
import type { NotificationItem } from '../api/types'
import { getErrorMessage } from '../api/errors'

export function NotificationBell() {
  const [open, setOpen] = useState(false)
  const [items, setItems] = useState<NotificationItem[]>([])
  const [error, setError] = useState('')
  const panelRef = useRef<HTMLDivElement>(null)

  const load = useCallback(async () => {
    try {
      setError('')
      const data = await listNotifications()
      setItems(data)
    } catch (err) {
      setError(getErrorMessage(err))
    }
  }, [])

  useEffect(() => {
    void load()
    const interval = setInterval(() => void load(), 60_000)
    return () => clearInterval(interval)
  }, [load])

  useEffect(() => {
    if (!open) return
    const onDoc = (e: MouseEvent) => {
      if (panelRef.current && !panelRef.current.contains(e.target as Node)) {
        setOpen(false)
      }
    }
    document.addEventListener('mousedown', onDoc)
    return () => document.removeEventListener('mousedown', onDoc)
  }, [open])

  const unread = items.filter((n) => !n.isRead).length

  const onMarkRead = async (id: string) => {
    await markNotificationRead(id)
    await load()
  }

  const preview = items.slice(0, 5)

  return (
    <div className="relative" ref={panelRef}>
      <button
        type="button"
        className="relative flex h-11 w-11 items-center justify-center rounded-lg text-slate-600 hover:bg-slate-100 sm:h-auto sm:w-auto sm:p-2"
        aria-label="Notifications"
        onClick={() => setOpen((v) => !v)}
      >
        <Bell className="h-5 w-5" />
        {unread > 0 && (
          <span className="absolute right-1 top-1 flex h-4 min-w-4 items-center justify-center rounded-full bg-rose-500 px-1 text-[10px] font-bold text-white">
            {unread > 9 ? '9+' : unread}
          </span>
        )}
      </button>

      {open && (
        <div className="fixed left-3 right-3 top-[4.5rem] z-50 max-h-[min(24rem,70vh)] overflow-auto rounded-xl border border-slate-200 bg-white shadow-lg sm:absolute sm:inset-x-auto sm:left-auto sm:right-0 sm:top-full sm:mt-2 sm:w-80 sm:max-h-96">
          <div className="flex items-center justify-between border-b border-slate-100 px-4 py-2">
            <span className="text-sm font-semibold text-slate-800">Notifications</span>
            <Link
              to="/notifications"
              className="text-xs font-medium text-blue-600 hover:text-blue-800"
              onClick={() => setOpen(false)}
            >
              View all
            </Link>
          </div>
          {error && <p className="px-4 py-2 text-xs text-red-600">{error}</p>}
          {preview.length === 0 ? (
            <p className="px-4 py-6 text-sm text-slate-500">No notifications yet.</p>
          ) : (
            <ul className="divide-y divide-slate-100">
              {preview.map((n) => (
                <li key={n.id} className={`px-4 py-3 text-sm ${n.isRead ? 'opacity-70' : ''}`}>
                  <p className="text-slate-800">{n.message}</p>
                  <p className="mt-1 text-xs text-slate-500">
                    {new Date(n.createdAt).toLocaleString()}
                  </p>
                  <div className="mt-2 flex gap-2">
                    {n.relatedTaskId && (
                      <Link
                        to={`/tasks/${n.relatedTaskId}`}
                        className="text-xs font-medium text-blue-600"
                        onClick={() => setOpen(false)}
                      >
                        View task
                      </Link>
                    )}
                    {!n.isRead && (
                      <button
                        type="button"
                        className="text-xs font-medium text-slate-600 hover:text-slate-900"
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
      )}
    </div>
  )
}
