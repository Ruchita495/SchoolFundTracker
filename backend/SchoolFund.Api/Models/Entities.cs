namespace SchoolFund.Api.Models;

public class AppUser { public int Id { get; set; } public string Username { get; set; } = ""; public string PasswordHash { get; set; } = ""; public string Role { get; set; } = "Treasurer"; }
public class Fund { public int Id { get; set; } public string Name { get; set; } = ""; public string? Description { get; set; } public decimal AmountPerStudent { get; set; } public DateTime DueDate { get; set; } }
public class Student { public int Id { get; set; } public string FirstName { get; set; } = ""; public string LastName { get; set; } = ""; public int Grade { get; set; } public string? GuardianEmail { get; set; } }
public class Payment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int FundId { get; set; }
    public Fund? Fund { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = "Cash";
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public string RecordedBy { get; set; } = "";
}
// Read-only report shapes
public record FundSummary(int FundId, string FundName, decimal Expected, decimal Collected, decimal Outstanding);
public record StudentBalance(int StudentId, string StudentName, int FundId, string FundName, decimal Owed, decimal Paid, decimal Balance, string Status);
