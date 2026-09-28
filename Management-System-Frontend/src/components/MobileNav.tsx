import { useEffect } from 'react'
import { NavLink } from 'react-router-dom'
import { X } from 'lucide-react'
import type { NavItem } from '../nav/navConfig'
import { navLinkActive } from '../styles/buttons'

interface MobileNavProps {
  open: boolean
  onClose: () => void
  items: NavItem[]
}

const linkClass = ({ isActive }: { isActive: boolean }) =>
  `flex min-h-[44px] items-center rounded-lg px-3 py-2.5 text-sm font-medium ${
    isActive ? navLinkActive : 'text-slate-700 hover:bg-slate-100'
  }`

export function MobileNav({ open, onClose, items }: MobileNavProps) {
  useEffect(() => {
    if (!open) return
    document.body.style.overflow = 'hidden'
    return () => {
      document.body.style.overflow = ''
    }
  }, [open])

  if (!open) return null

  return (
    <div className="fixed inset-0 z-50 sm:hidden">
      <button
        type="button"
        className="absolute inset-0 bg-slate-900/40"
        aria-label="Close menu"
        onClick={onClose}
      />
      <aside className="absolute left-0 top-0 flex h-full w-[min(280px,85vw)] flex-col bg-white shadow-xl">
        <div className="flex items-center justify-between border-b border-slate-200 px-4 py-3">
          <span className="text-lg font-semibold text-slate-900">TaskFlow</span>
          <button
            type="button"
            className="rounded-lg p-2 text-slate-600 hover:bg-slate-100"
            aria-label="Close"
            onClick={onClose}
          >
            <X className="h-5 w-5" />
          </button>
        </div>
        <nav className="flex flex-col gap-1 p-3" aria-label="Mobile">
          {items.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              end={item.end}
              className={linkClass}
              onClick={onClose}
            >
              {item.label}
            </NavLink>
          ))}
          <NavLink to="/notifications" className={linkClass} onClick={onClose}>
            Notifications
          </NavLink>
          <NavLink to="/settings" className={linkClass} onClick={onClose}>
            Settings
          </NavLink>
        </nav>
      </aside>
    </div>
  )
}
