using Microsoft.EntityFrameworkCore;
using SchoolFund.Api.Models;

namespace SchoolFund.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Fund> Funds => Set<Fund>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<AppUser>().HasIndex(u => u.Username).IsUnique();
        b.Entity<Fund>().Property(f => f.AmountPerStudent).HasPrecision(10, 2);
        b.Entity<Payment>().Property(p => p.Amount).HasPrecision(10, 2);
        b.Entity<Payment>().HasIndex(p => new { p.StudentId, p.FundId });
    }
}
