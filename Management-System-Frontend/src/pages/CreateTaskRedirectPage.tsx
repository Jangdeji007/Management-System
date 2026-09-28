import { Navigate } from 'react-router-dom'

/** Legacy route: opens create modal on the tasks list. */
export function CreateTaskRedirectPage() {
  return <Navigate to="/tasks?create=1" replace />
}
