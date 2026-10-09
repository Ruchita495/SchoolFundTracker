using HotChocolate.AspNetCore;
using HotChocolate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using SchoolFund.Api.Data;
using SchoolFund.Api.GraphQL;
using SchoolFund.Api.Models;
using SchoolFund.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5050");

builder.Services.AddDbContextFactory<AppDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped(sp => sp.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());
builder.Services.AddScoped<PaymentService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
    o.TokenValidationParameters = new()
    {
        ValidateIssuer = false, ValidateAudience = false, ValidateLifetime = true,
        IssuerSigningKey = AuthService.Key(builder.Configuration)
    });
builder.Services.AddAuthorization();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddGraphQLServer()
    .AddAuthorization()
    .ModifyRequestOptions(o => o.IncludeExceptionDetails = true)   // shows the real error; set to false in production
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var auth = scope.ServiceProvider.GetRequiredService<AuthService>();
    try { db.Database.EnsureCreated(); }
    catch (Exception e)
    {
        Console.WriteLine("\nCANNOT CONNECT TO SQL SERVER. Check ConnectionStrings:Default in appsettings.json.\n" + e.Message);
        return;
    }
    if (!db.Users.Any())
    {
        foreach (var (name, pw, role) in new[] { ("admin", "Admin123!", "Admin"), ("treasurer", "Treasurer123!", "Treasurer") })
        { var u = new AppUser { Username = name, Role = role }; u.PasswordHash = auth.Hash(u, pw); db.Users.Add(u); }
    }
    if (!db.Funds.Any())
        db.Funds.AddRange(new Fund { Name = "Field Trip", Description = "Science centre", AmountPerStudent = 25, DueDate = DateTime.Today.AddDays(30) },
                          new Fund { Name = "Yearbook", AmountPerStudent = 40, DueDate = DateTime.Today.AddDays(60) });
    if (!db.Students.Any())
        db.Students.AddRange(new Student { FirstName = "Aarav", LastName = "Shah", Grade = 5 },
                             new Student { FirstName = "Maya", LastName = "Singh", Grade = 5 },
                             new Student { FirstName = "Liam", LastName = "Brown", Grade = 6 });
    db.SaveChanges();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/login", async (LoginRequest req, AppDbContext db, AuthService auth) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
    if (user is null || !auth.Verify(user, req.Password)) return Results.Unauthorized();
    return Results.Ok(new { token = auth.CreateToken(user), role = user.Role, username = user.Username });
});
app.MapGet("/", () => Results.Text("School Fund API is running. GraphQL playground: /graphql | Health check: /health"));
app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTime.UtcNow }));
app.MapGraphQL().WithOptions(new GraphQLServerOptions { Tool = { Enable = true } });   // /graphql (playground in browser)
app.Run();

public record LoginRequest(string Username, string Password);
public partial class Program { }
