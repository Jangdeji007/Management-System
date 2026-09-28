import type { UserRoleName } from '../api/enums'
import { UserRole } from '../api/enums'

export interface NavItem {
  to: string
  label: string
  end?: boolean
}

export function navItemsForRole(role: UserRoleName): NavItem[] {
  const tasksLabel = role === UserRole.User ? 'My Tasks' : 'Tasks'

  if (role === UserRole.Admin) {
    return [
      { to: '/', label: 'Dashboard', end: true },
      { to: '/users', label: 'Users' },
      { to: '/teams', label: 'Teams' },
      { to: '/tasks', label: tasksLabel },
    ]
  }

  if (role === UserRole.Manager) {
    return [
      { to: '/', label: 'Dashboard', end: true },
      { to: '/teams', label: 'My Team' },
      { to: '/tasks', label: tasksLabel },
    ]
  }

  return [
    { to: '/', label: 'Dashboard', end: true },
    { to: '/tasks', label: tasksLabel },
  ]
}
