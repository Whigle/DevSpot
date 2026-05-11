# DevSpot

An ASP.NET Core MVC web application for managing job postings.

## Features

- Full CRUD for job offers (create, read, update, delete)
- Filtering job postings
- Repository pattern (`IRepository<T>` + `JobPostingRepository`)
- Entity Framework Core with Code First migrations
- ASP.NET Core Identity (authentication & authorization)
- ViewModels and Areas
- Unit test project (`DevSpot.Tests`)

## Tech Stack

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- SQL Server / LocalDB
- ASP.NET Core Identity
- xUnit

## Getting Started

```bash
# 1. Apply database migrations
dotnet ef database update --project DevSpot

# 2. Run the application
dotnet run --project DevSpot
```

Port is assigned automatically — check the console output after `dotnet run` for the exact URL.
