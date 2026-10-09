import { useState } from 'react';
import { login } from '../api';
import type { Session } from '../types';

export default function Login({ onLogin }: { onLogin: (s: Session) => void }) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    setBusy(true); setError('');
    try { onLogin(await login(username, password)); }
    catch (err) { setError((err as Error).message); }
    finally { setBusy(false); }
  }

  return (
    <main><form className="card" onSubmit={submit} style={{ maxWidth: 360, margin: '4rem auto' }}>
      <h1>Activity Fund Tracker</h1>
      <label htmlFor="u">Username</label><input id="u" value={username} onChange={e => setUsername(e.target.value)} required />
      <label htmlFor="p">Password</label><input id="p" type="password" value={password} onChange={e => setPassword(e.target.value)} required />
      {error && <p role="alert" className="error">{error}</p>}
      <button disabled={busy}>{busy ? 'Signing in…' : 'Sign in'}</button>
    </form></main>
  );
}
