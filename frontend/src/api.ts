import type { Session } from './types';

export async function login(username: string, password: string): Promise<Session> {
  const r = await fetch('/api/login', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ username, password }) });
  if (!r.ok) throw new Error('Invalid username or password');
  return r.json();
}

export async function gql<T>(query: string, variables?: Record<string, unknown>): Promise<T> {
  const token = localStorage.getItem('token');
  const r = await fetch('/graphql', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    body: JSON.stringify({ query, variables }),
  });
  const json = await r.json();
  if (json.errors?.length) throw new Error(json.errors[0].message);
  return json.data as T;
}
