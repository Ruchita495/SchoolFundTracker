using System.Security.Claims;
using HotChocolate;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using SchoolFund.Api.Data;
using SchoolFund.Api.Models;
using SchoolFund.Api.Services;

namespace SchoolFund.Api.GraphQL;

public class Mutation
{
    // Treasurers and Admins can record payments
    [Authorize(Roles = new[] { "Admin", "Treasurer" })]
    public async Task<Payment> RecordPayment([Service] IDbContextFactory<AppDbContext> factory, ClaimsPrincipal user,
        int studentId, int fundId, decimal amount, string method)
    {
        await using var db = await factory.CreateDbContextAsync();
        try { return await new PaymentService(db).RecordAsync(studentId, fundId, amount, method, user.Identity?.Name ?? "unknown"); }
        catch (Exception e) when (e is ArgumentException or KeyNotFoundException or InvalidOperationException)
        { throw new GraphQLException(e.Message); }
    }

    // Only Admins manage funds and students
    [Authorize(Roles = new[] { "Admin" })]
    public async Task<Fund> CreateFund([Service] IDbContextFactory<AppDbContext> factory, string name, string? description, decimal amountPerStudent, DateTime dueDate)
    {
        if (string.IsNullOrWhiteSpace(name) || amountPerStudent <= 0) throw new GraphQLException("Name and a positive amount are required.");
        await using var db = await factory.CreateDbContextAsync();
        var fund = new Fund { Name = name.Trim(), Description = description, AmountPerStudent = amountPerStudent, DueDate = dueDate };
        db.Funds.Add(fund); await db.SaveChangesAsync(); return fund;
    }

    [Authorize(Roles = new[] { "Admin" })]
    public async Task<Student> AddStudent([Service] IDbContextFactory<AppDbContext> factory, string firstName, string lastName, int grade, string? guardianEmail)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName)) throw new GraphQLException("Name is required.");
        await using var db = await factory.CreateDbContextAsync();
        var s = new Student { FirstName = firstName.Trim(), LastName = lastName.Trim(), Grade = grade, GuardianEmail = guardianEmail };
        db.Students.Add(s); await db.SaveChangesAsync(); return s;
    }
}
