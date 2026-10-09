import type { Summary } from '../types';

const money = (n: number) => n.toLocaleString('en-CA', { style: 'currency', currency: 'CAD' });

export default function SummaryCards({ items }: { items: Summary[] }) {
  if (items.length === 0) return <p>No funds yet.</p>;
  return (
    <div className="cards">
      {items.map(s => (
        <div className="card" key={s.fundId}>
          <h3>{s.fundName}</h3>
          <progress value={s.collected} max={s.expected || 1} />
          <p>Collected {money(s.collected)} of {money(s.expected)}</p>
          <p>Outstanding: <strong>{money(s.outstanding)}</strong></p>
        </div>
      ))}
    </div>
  );
}
