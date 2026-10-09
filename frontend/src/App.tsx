import { useCallback, useEffect, useState } from 'react';
import { gql } from './api';
import Login from './components/Login';
import SummaryCards from './components/SummaryCards';
import BalanceTable from './components/BalanceTable';
import PaymentForm from './components/PaymentForm';
import type { Balance, Fund, Payment, Session, Student, Summary } from './types';

const QUERY = `{
  funds { id name amountPerStudent }
  students { id firstName lastName }
  balances { studentId studentName fundId fundName owed paid balance status }
  fundSummaries { fundId fundName expected collected outstanding }
  payments { id amount method paidAt recordedBy student { firstName lastName } fund { name } }
}`;

interface Data { funds: Fund[]; students: Student[]; balances: Balance[]; fundSummaries: Summary[]; payments: Payment[] }

export default function App() {
  const [session, setSession] = useState<Session | null>(() => {
    const t = localStorage.getItem('token'), s = localStorage.getItem('session');
    return t && s ? JSON.parse(s) : null;
  });
  const [data, setData] = useState<Data | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const logout = () => { localStorage.clear(); setSession(null); setData(null); };

  const load = useCallback(async () => {
    setLoading(true); setError('');
    try { setData(await gql<Data>(QUERY)); }
    catch (e) { setError((e as Error).message); }
    finally { setLoading(false); }
  }, []);

  useEffect(() => { if (session) load(); }, [session, load]);

  if (!session) return <Login onLogin={s => { localStorage.setItem('token', s.token); localStorage.setItem('session', JSON.stringify(s)); setSession(s); }} />;

  return (
    <main>
      <header><h1>School Activity Fund Tracker</h1><div>{session.username} ({session.role}) <button onClick={logout}>Log out</button></div></header>
      {loading && <p>Loading…</p>}
      {error && <p role="alert" className="error">{error}</p>}
      {data && <>
        <h2>Fund Summary</h2>
        <SummaryCards items={data.fundSummaries} />
        <PaymentForm students={data.students} funds={data.funds} onSaved={load} />
        <BalanceTable rows={data.balances} />
        <div className="card"><h2>Recent Transactions</h2>
          <table><thead><tr><th>Date</th><th>Student</th><th>Fund</th><th>Amount</th><th>Method</th><th>By</th></tr></thead>
            <tbody>{data.payments.map(p => (
              <tr key={p.id}><td>{new Date(p.paidAt).toLocaleDateString()}</td><td>{p.student.firstName} {p.student.lastName}</td>
                <td>{p.fund.name}</td><td>${p.amount.toFixed(2)}</td><td>{p.method}</td><td>{p.recordedBy}</td></tr>))}
              {data.payments.length === 0 && <tr><td colSpan={6}>No payments yet.</td></tr>}</tbody></table></div>
      </>}
    </main>
  );
}
