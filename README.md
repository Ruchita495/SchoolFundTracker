# School Activity Fund & Payment Tracker

React + TypeScript frontend, ASP.NET Core (C#) backend, GraphQL API (HotChocolate), SQL Server database,
xUnit + Playwright tests.

## How the pieces fit together
Browser (React) --GraphQL/JSON--> ASP.NET Core API --Entity Framework--> SQL Server

## Prerequisites
.NET 8 SDK, Node 20+, SQL Server (Windows: LocalDB comes with Visual Studio; Mac/Linux: run SQL Server in Docker
and edit the connection string in backend/SchoolFund.Api/appsettings.json).

## Run it
1. Backend:   cd backend/SchoolFund.Api && dotnet run          (http://localhost:5050, GraphQL at /graphql)
2. Frontend:  cd frontend && npm install && npm run dev        (http://localhost:5173)
   Check the API: open http://localhost:5050/health (should show {"status":"ok"}).
3. Log in:    admin / Admin123!   or   treasurer / Treasurer123!   (change these before real use!)

The database and sample data are created automatically on first run. database/schema.sql is the same schema as plain SQL.

## Tests
- Backend unit tests:  cd backend/SchoolFund.Tests && dotnet test
- End-to-end:          (backend + frontend running) cd frontend && npx playwright install chromium && npm run e2e

## Build order if you want to learn by rebuilding
1. database/schema.sql      - tables and relationships
2. Models + AppDbContext    - C# classes mapped to tables
3. PaymentService           - business rules (validation, balances)
4. GraphQL Query/Mutation   - the API surface
5. Program.cs               - wiring, JWT login, seeding
6. React: api.ts -> Login -> App -> components
7. Tests

## Git workflow (as in your resume)
git init; git checkout -b feature/payments; git add .; git commit -m "Add payment recording"; open a Pull Request on GitHub.
