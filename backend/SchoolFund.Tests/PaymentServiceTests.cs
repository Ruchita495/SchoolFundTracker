using Microsoft.EntityFrameworkCore;
using SchoolFund.Api.Data;
using SchoolFund.Api.Models;
using SchoolFund.Api.Services;

namespace SchoolFund.Tests;

public class PaymentServiceTests
{
    private static (AppDbContext db, PaymentService svc) Setup()
    {
        var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        db.Funds.Add(new Fund { Id = 1, Name = "Trip", AmountPerStudent = 25, DueDate = DateTime.Today });
        db.Students.Add(new Student { Id = 1, FirstName = "A", LastName = "B", Grade = 5 });
        db.SaveChanges();
        return (db, new PaymentService(db));
    }

    [Fact] public async Task Records_valid_payment()
    {
        var (_, svc) = Setup();
        var p = await svc.RecordAsync(1, 1, 10, "Cash", "admin");
        Assert.Equal(10, p.Amount);
    }

    [Theory][InlineData(0)][InlineData(-5)]
    public async Task Rejects_non_positive_amount(decimal amount)
    {
        var (_, svc) = Setup();
        await Assert.ThrowsAsync<ArgumentException>(() => svc.RecordAsync(1, 1, amount, "Cash", "admin"));
    }

    [Fact] public async Task Rejects_overpayment()
    {
        var (_, svc) = Setup();
        await svc.RecordAsync(1, 1, 20, "Cash", "admin");
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.RecordAsync(1, 1, 10, "Card", "admin"));
    }

    [Fact] public async Task Balance_status_moves_Unpaid_Partial_Paid()
    {
        var (_, svc) = Setup();
        Assert.Equal("Unpaid", (await svc.BalancesAsync()).Single().Status);
        await svc.RecordAsync(1, 1, 10, "Cash", "admin");
        Assert.Equal("Partial", (await svc.BalancesAsync()).Single().Status);
        await svc.RecordAsync(1, 1, 15, "Cash", "admin");
        Assert.Equal("Paid", (await svc.BalancesAsync()).Single().Status);
    }
}
