export function LoadingSpinner({ label = 'Loading…' }: { label?: string }) {
  return (
    <div className="flex items-center justify-center gap-2 py-12 text-slate-600">
      <span
        className="inline-block h-5 w-5 animate-spin rounded-full border-2 border-slate-300 border-t-blue-600"
        aria-hidden
      />
      <span>{label}</span>
    </div>
  )
}
