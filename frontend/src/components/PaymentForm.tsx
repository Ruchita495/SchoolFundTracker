import { useState } from 'react';
import { gql } from '../api';
import type { Fund, Student } from '../types';

const MUTATION = `mutation($studentId:Int!,$fundId:Int!,$amount:Decimal!,$method:String!){
  recordPayment(studentId:$studentId,fundId:$fundId,amount:$amount,method:$method){ id } }`;

export default function PaymentForm({ students, funds, onSaved }: { students: Student[]; funds: Fund[]; onSaved: () => void }) {
  const [studentId, setStudentId] = useState('');
  const [fundId, setFundId] = useState('');
  const [amount, setAmount] = useState('');
  const [method, setMethod] = useState('Cash');
  const [msg, setMsg] = useState<{ ok: boolean; text: string } | null>(null);

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    setMsg(null);
    const amt = parseFloat(amount);
    if (!studentId || !fundId || !(amt > 0)) return setMsg({ ok: false, text: 'Choose a student, a fund and an amount above 0.' });
    try {
      await gql(MUTATION, { studentId: +studentId, fundId: +fundId, amount: amt, method });
      setMsg({ ok: true, text: 'Payment recorded' }); setAmount(''); onSaved();
    } catch (err) { setMsg({ ok: false, text: (err as Error).message }); }
  }

  return (
    <form className="card" onSubmit={submit}>
      <h2>Record Payment</h2>
      <label>Student <select value={studentId} onChange={e => setStudentId(e.target.value)}>
        <option value="">Select…</option>{students.map(s => <option key={s.id} value={s.id}>{s.firstName} {s.lastName}</option>)}</select></label>
      <label>Fund <select value={fundId} onChange={e => setFundId(e.target.value)}>
        <option value="">Select…</option>{funds.map(f => <option key={f.id} value={f.id}>{f.name}</option>)}</select></label>
      <label>Amount <input type="number" step="0.01" min="0" value={amount} onChange={e => setAmount(e.target.value)} /></label>
      <label>Method <select value={method} onChange={e => setMethod(e.target.value)}>
        {['Cash', 'Cheque', 'Card', 'E-transfer'].map(m => <option key={m}>{m}</option>)}</select></label>
      <button>Record payment</button>
      {msg && <p className={msg.ok ? 'ok' : 'error'} role={msg.ok ? 'status' : 'alert'}>{msg.text}</p>}
    </form>
  );
}
