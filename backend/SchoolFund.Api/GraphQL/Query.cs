using HotChocolate;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using SchoolFund.Api.Data;
using SchoolFund.Api.Models;
using SchoolFund.Api.Services;

namespace SchoolFund.Api.GraphQL;

// Each resolver creates its OWN DbContext from the factory, because GraphQL runs fields in parallel
// and one DbContext cannot be used by two operations at the same time.
[Authorize]
public class Query
{
    public async Task<List<Fund>> GetFunds([Service] IDbContextFactory<AppDbContext> factory)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Funds.OrderBy(f => f.DueDate).ToListAsync();
    }

    public async Task<List<Student>> GetStudents([Service] IDbContextFactory<AppDbContext> factory)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Students.OrderBy(s => s.LastName).ToListAsync();
    }

    public async Task<List<Payment>> GetPayments([Service] IDbContextFactory<AppDbContext> factory, int? fundId)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Payments.Include(p => p.Student).Include(p => p.Fund)
            .Where(p => fundId == null || p.FundId == fundId)
            .OrderByDescending(p => p.PaidAt).Take(200).ToListAsync();
    }

    public async Task<List<StudentBalance>> GetBalances([Service] IDbContextFactory<AppDbContext> factory)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await new PaymentService(db).BalancesAsync();
    }

    public async Task<List<FundSummary>> GetFundSummaries([Service] IDbContextFactory<AppDbContext> factory)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await new PaymentService(db).SummariesAsync();
    }
}
