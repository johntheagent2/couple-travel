import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
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
    <div className="min-h-screen flex items-center justify-center bg-warm-50 px-4">
      <div className="w-full max-w-sm">
        <div className="text-center mb-8">
          <div className="text-5xl mb-3">🗺️</div>
          <h1 className="text-3xl font-bold text-warm-800" style={{ fontFamily: 'Georgia, serif' }}>
            Our Adventures
          </h1>
          <p className="text-warm-600 mt-1 text-sm">Your shared travel scrapbook</p>
        </div>

        <form onSubmit={submit} className="bg-white rounded-2xl shadow-sm border border-warm-100 p-6 space-y-4">
          <div>
            <label className="block text-sm font-medium text-warm-700 mb-1">Email</label>
            <input
              type="email"
              value={email}
              onChange={e => setEmail(e.target.value)}
              required
              className="w-full rounded-xl border border-warm-200 px-3 py-2 text-sm outline-none focus:border-warm-400 focus:ring-2 focus:ring-warm-100"
              placeholder="you@example.com"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-warm-700 mb-1">Password</label>
            <input
              type="password"
              value={password}
              onChange={e => setPassword(e.target.value)}
              required
              className="w-full rounded-xl border border-warm-200 px-3 py-2 text-sm outline-none focus:border-warm-400 focus:ring-2 focus:ring-warm-100"
            />
          </div>
          {error && <p className="text-red-500 text-sm">{error}</p>}
          <button
            type="submit"
            disabled={login.isPending}
            className="w-full bg-warm-500 hover:bg-warm-600 text-white rounded-xl py-2.5 font-medium text-sm transition-colors disabled:opacity-60"
          >
            {login.isPending ? 'Signing in…' : 'Sign in'}
          </button>
        </form>
      </div>
    </div>
  )
}
