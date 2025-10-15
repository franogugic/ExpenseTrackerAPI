# ExpenseTrackerAPI

Framework: .NET 9 (C#)  
Database: SQL Server (Docker)  
ORM: Entity Framework Core  
Authentication: JWT + BCrypt  
Validation: FluentValidation  
Documentation: Swagger

---

## Project Structure

ExpenseTrackerAPI/
- Controllers: API controllers (User, Expense)
- Data: EF Core DbContext
- Models: Entities + DTOs
- Services: Business logic
- Validators: FluentValidation validators
- Migrations: EF migrations
- appsettings.json: Configuration (DB + JWT)
- Program.cs: Service registration + middleware pipeline

---

## Features Implemented

- User registration and login with JWT authentication
- CRUD operations for Expenses (Create, Read, Update, Delete)
- Filtering expenses by time period, custom date range, category, and sorting
- Dates formatted as `dd.MM.yyyy` in responses
- Global Error Handler middleware for consistent JSON error responses
- Swagger UI with JWT support

---

## Getting Started

1. Clone the repository
2. Configure SQL Server in Docker or locally
3. Update connection string in `appsettings.json`
4. Run migrations to create the database
5. Launch the API and test endpoints via Swagger
