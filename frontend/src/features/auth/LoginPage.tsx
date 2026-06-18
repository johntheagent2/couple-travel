import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Compass, Heart } from 'lucide-react'
import { useLogin } from './useAuth'

export function LoginPage() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const navigate = useNavigate()
  const login = useLogin()

  const submit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError('')
    try {
      await login.mutateAsync({ email, password })
      navigate('/')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Login failed')
    }
  }

  return (
    <div className="min-h-screen flex flex-col bg-warm-50" style={{
      backgroundImage: 'radial-gradient(ellipse 100% 50% at 50% -5%, rgba(220, 143, 48, 0.15), transparent 70%)',
    }}>
      {/* Journal cover */}
      <div className="flex-1 flex flex-col items-center justify-center px-6 pt-16 pb-8">
        <div className="text-center mb-10">
          <div className="inline-flex items-center justify-center w-16 h-16 rounded-2xl bg-warm-100 text-warm-500 mb-6" aria-hidden>
            <Compass size={32} strokeWidth={1.5} />
          </div>
          <h1 className="font-display text-4xl font-semibold text-warm-900 tracking-tight leading-tight">
            Our Adventures
          </h1>
          <p className="mt-3 text-warm-500 text-sm tracking-wide">
            your shared travel journal
          </p>
        </div>

        <form onSubmit={submit} className="w-full max-w-xs space-y-4">
          <div>
            <label className="block text-xs font-medium text-warm-600 mb-1.5 tracking-wide uppercase">Email</label>
            <input
              type="email"
              value={email}
              onChange={e => setEmail(e.target.value)}
              required
              autoComplete="email"
              className="w-full rounded-xl border border-warm-200 bg-white px-4 py-2.5 text-sm text-warm-900 outline-none placeholder:text-warm-300 focus:border-warm-400 focus:ring-2 focus:ring-warm-100 transition-shadow"
              placeholder="you@example.com"
            />
          </div>
          <div>
            <label className="block text-xs font-medium text-warm-600 mb-1.5 tracking-wide uppercase">Password</label>
            <input
              type="password"
              value={password}
              onChange={e => setPassword(e.target.value)}
              required
              autoComplete="current-password"
              className="w-full rounded-xl border border-warm-200 bg-white px-4 py-2.5 text-sm text-warm-900 outline-none placeholder:text-warm-300 focus:border-warm-400 focus:ring-2 focus:ring-warm-100 transition-shadow"
            />
          </div>
          {error && <p className="text-red-500 text-sm">{error}</p>}
          <button
            type="submit"
            disabled={login.isPending}
            className="w-full bg-warm-500 hover:bg-warm-600 active:bg-warm-700 text-white rounded-xl py-2.5 font-medium text-sm transition-colors disabled:opacity-60 mt-2"
          >
            {login.isPending ? 'Signing in…' : 'Open journal'}
          </button>
        </form>
      </div>

      <p className="text-center text-warm-300 text-xs pb-8 select-none flex items-center justify-center gap-1.5">
        made with <Heart size={10} className="text-warm-400 fill-warm-400" />
      </p>
    </div>
  )
}
