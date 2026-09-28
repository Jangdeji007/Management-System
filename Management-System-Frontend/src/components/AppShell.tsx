import { useState } from 'react'
import { Link, NavLink, Outlet } from 'react-router-dom'
import { Menu } from 'lucide-react'
import { useAuth } from '../auth/AuthContext'
import { navItemsForRole } from '../nav/navConfig'
import { navLinkActive } from '../styles/buttons'
import { MobileNav } from './MobileNav'
import { NotificationBell } from './NotificationBell'
import { UserMenu } from './UserMenu'

const linkClass = ({ isActive }: { isActive: boolean }) =>
  `hidden rounded-lg px-3 py-2 text-sm font-medium sm:inline-flex sm:items-center ${
    isActive ? navLinkActive : 'text-slate-600 hover:bg-slate-100'
  }`

export function AppShell() {
  const { user } = useAuth()
  const [mobileOpen, setMobileOpen] = useState(false)
  const items = user ? navItemsForRole(user.role) : []

  return (
    <div className="flex min-h-screen flex-col">
      <header className="sticky top-0 z-40 border-b border-slate-200 bg-white/95 backdrop-blur supports-[backdrop-filter]:bg-white/80">
        <div className="page-container !py-3 sm:!py-4">
          <div className="flex items-center justify-between gap-3">
            <div className="flex min-w-0 items-center gap-2 sm:gap-4">
              <button
                type="button"
                className="rounded-lg p-2 text-slate-600 hover:bg-slate-100 sm:hidden"
                aria-label="Open menu"
                onClick={() => setMobileOpen(true)}
              >
                <Menu className="h-6 w-6" />
              </button>
              <Link to="/" className="truncate text-base font-semibold text-slate-900 sm:text-lg">
                TaskFlow
              </Link>
              <nav className="hidden min-w-0 flex-wrap gap-1 sm:flex" aria-label="Main">
                {items.map((item) => (
                  <NavLink key={item.to} to={item.to} end={item.end} className={linkClass}>
                    {item.label}
                  </NavLink>
                ))}
              </nav>
            </div>

            <div className="flex shrink-0 items-center gap-1 sm:gap-2">
              <NotificationBell />
              <UserMenu />
            </div>
          </div>
        </div>
      </header>

      <MobileNav open={mobileOpen} onClose={() => setMobileOpen(false)} items={items} />

      <main className="page-container flex-1">
        <Outlet />
      </main>
    </div>
  )
}
