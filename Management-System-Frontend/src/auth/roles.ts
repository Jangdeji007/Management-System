import { UserRole, type UserRoleName } from '../api/enums'

export function isAdmin(role: UserRoleName): boolean {
  return role === UserRole.Admin
}

export function isManagerOrAdmin(role: UserRoleName): boolean {
  return role === UserRole.Admin || role === UserRole.Manager
}
