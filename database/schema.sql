-- School Activity Fund Tracker: tables only.
-- BEFORE running: create an empty database named SchoolFundDb and select it
-- (SSMS / Azure Data Studio: right-click Databases > New Database, then pick it in the dropdown).
-- Safe to run more than once (no GO needed). Note: dropping tables deletes their data.

IF OBJECT_ID('dbo.vw_StudentBalances', 'V') IS NOT NULL DROP VIEW dbo.vw_StudentBalances;
IF OBJECT_ID('dbo.Payments', 'U') IS NOT NULL DROP TABLE dbo.Payments;
IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL DROP TABLE dbo.Students;
IF OBJECT_ID('dbo.Funds', 'U') IS NOT NULL DROP TABLE dbo.Funds;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;

CREATE TABLE dbo.Users (
  Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  Username NVARCHAR(50) NOT NULL UNIQUE,
  PasswordHash NVARCHAR(MAX) NOT NULL,
  Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin','Treasurer'))
);

CREATE TABLE dbo.Funds (
  Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  Name NVARCHAR(100) NOT NULL,
  Description NVARCHAR(500) NULL,
  AmountPerStudent DECIMAL(10,2) NOT NULL CHECK (AmountPerStudent > 0),
  DueDate DATETIME2 NOT NULL
);

CREATE TABLE dbo.Students (
  Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  FirstName NVARCHAR(50) NOT NULL,
  LastName NVARCHAR(50) NOT NULL,
  Grade INT NOT NULL,
  GuardianEmail NVARCHAR(150) NULL
);

CREATE TABLE dbo.Payments (
  Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  StudentId INT NOT NULL REFERENCES dbo.Students(Id),
  FundId INT NOT NULL REFERENCES dbo.Funds(Id),
  Amount DECIMAL(10,2) NOT NULL CHECK (Amount > 0),
  Method NVARCHAR(20) NOT NULL,
  PaidAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  RecordedBy NVARCHAR(50) NOT NULL
);

CREATE INDEX IX_Payments_StudentId_FundId ON dbo.Payments(StudentId, FundId);

-- Optional sample data (users are created by the app on first run, because passwords must be hashed by C#)
INSERT INTO dbo.Funds (Name, Description, AmountPerStudent, DueDate) VALUES
 ('Field Trip', 'Science centre', 25.00, DATEADD(day, 30, SYSUTCDATETIME())),
 ('Yearbook', NULL, 40.00, DATEADD(day, 60, SYSUTCDATETIME()));
INSERT INTO dbo.Students (FirstName, LastName, Grade) VALUES
 ('Aarav','Shah',5), ('Maya','Singh',5), ('Liam','Brown',6);

-- Reconciliation report as a view. EXEC() makes SQL Server check the columns at run time,
-- after the tables above exist (this avoids "Invalid column name" errors).
EXEC('
CREATE VIEW dbo.vw_StudentBalances AS
SELECT s.Id AS StudentId, s.FirstName, s.LastName, f.Id AS FundId, f.Name AS Fund,
       f.AmountPerStudent AS Owed,
       ISNULL(SUM(p.Amount), 0) AS Paid,
       f.AmountPerStudent - ISNULL(SUM(p.Amount), 0) AS Balance
FROM dbo.Students s
CROSS JOIN dbo.Funds f
LEFT JOIN dbo.Payments p ON p.StudentId = s.Id AND p.FundId = f.Id
GROUP BY s.Id, s.FirstName, s.LastName, f.Id, f.Name, f.AmountPerStudent
');

EXEC('SELECT * FROM dbo.vw_StudentBalances');
