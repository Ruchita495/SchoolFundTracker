using Microsoft.EntityFrameworkCore;
using SchoolFund.Api.Data;
using SchoolFund.Api.Models;

namespace SchoolFund.Api.Services;

/// Business rules live here (not in GraphQL) so they are easy to unit test.
public class PaymentService(AppDbContext db)
{
    public async Task<Payment> RecordAsync(int studentId, int fundId, decimal amount, string method, string recordedBy)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.");
        var fund = await db.Funds.FindAsync(fundId) ?? throw new KeyNotFoundException("Fund not found.");
        if (!await db.Students.AnyAsync(s => s.Id == studentId)) throw new KeyNotFoundException("Student not found.");

        var paid = (await db.Payments.Where(p => p.StudentId == studentId && p.FundId == fundId)
                          .Select(p => p.Amount).ToListAsync()).Sum();
        if (paid + amount > fund.AmountPerStudent)
            throw new InvalidOperationException($"Payment exceeds amount owed. Remaining: {fund.AmountPerStudent - paid:0.00}");

        var payment = new Payment { StudentId = studentId, FundId = fundId, Amount = amount, Method = method, RecordedBy = recordedBy, PaidAt = DateTime.UtcNow };
        db.Payments.Add(payment);
        await db.SaveChangesAsync();
        return payment;
    }

    public async Task<List<StudentBalance>> BalancesAsync()
    {
        var funds = await db.Funds.ToListAsync();
        var students = await db.Students.ToListAsync();
        var paid = (await db.Payments.ToListAsync())
            .GroupBy(p => (p.StudentId, p.FundId)).ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));

        return (from f in funds from s in students
                let p = paid.GetValueOrDefault((s.Id, f.Id))
                select new StudentBalance(s.Id, $"{s.FirstName} {s.LastName}", f.Id, f.Name, f.AmountPerStudent, p,
                    f.AmountPerStudent - p, p == 0 ? "Unpaid" : p >= f.AmountPerStudent ? "Paid" : "Partial")).ToList();
    }

    public async Task<List<FundSummary>> SummariesAsync()
    {
        var balances = await BalancesAsync();
        return balances.GroupBy(b => (b.FundId, b.FundName))
            .Select(g => new FundSummary(g.Key.FundId, g.Key.FundName, g.Sum(b => b.Owed), g.Sum(b => b.Paid), g.Sum(b => b.Balance))).ToList();
    }
}
