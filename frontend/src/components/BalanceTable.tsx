import { useMemo, useState } from 'react';
import type { Balance } from '../types';

export default function BalanceTable({ rows }: { rows: Balance[] }) {
  const [search, setSearch] = useState('');
  const [status, setStatus] = useState('All');

  const filtered = useMemo(() => rows.filter(r =>
    r.studentName.toLowerCase().includes(search.toLowerCase()) && (status === 'All' || r.status === status)), [rows, search, status]);

  return (
    <div className="card">
      <h2>Student Balances</h2>
      <input placeholder="Search student…" aria-label="Search student" value={search} onChange={e => setSearch(e.target.value)} />{' '}
      <select aria-label="Filter by status" value={status} onChange={e => setStatus(e.target.value)}>
        {['All', 'Paid', 'Partial', 'Unpaid'].map(s => <option key={s}>{s}</option>)}
      </select>
      <table>
        <thead><tr><th>Student</th><th>Fund</th><th>Owed</th><th>Paid</th><th>Balance</th><th>Status</th></tr></thead>
        <tbody>
          {filtered.map(r => (
            <tr key={`${r.studentId}-${r.fundId}`}>
              <td>{r.studentName}</td><td>{r.fundName}</td><td>${r.owed.toFixed(2)}</td><td>${r.paid.toFixed(2)}</td>
              <td>${r.balance.toFixed(2)}</td><td><span className={`badge ${r.status}`}>{r.status}</span></td>
            </tr>
          ))}
          {filtered.length === 0 && <tr><td colSpan={6}>No matching records.</td></tr>}
        </tbody>
      </table>
    </div>
  );
}
