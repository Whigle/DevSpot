# DevSpot

DevSpot is a job board web application for developers built with ASP.NET Core MVC and Entity Framework Core.

The platform allows employers to create company profiles and publish job offers, while job seekers can browse listings and apply with their CVs.

---

## Features

### Authentication & Authorization
- ASP.NET Core Identity authentication
- Role-based authorization
- User roles: Admin, Employer, Job Seeker
- Optional email confirmation flow

### Job Offers
- Create, edit and delete job postings
- Pagination support
- Filtering and sorting

### Company Profiles
- Employers can create a company profile
- One company profile per employer account

### Applications System
- Apply to job offers
- CV upload support (.pdf only)
- File validation: PDF format only, max size 5MB
- Prevents duplicate applications

### Seeded Demo Data
- Seeded users
- Seeded roles
- Fake companies and job offers generated using Bogus

### Error Handling
- Custom 404 page
- Custom 500 page

---

## Technologies

- ASP.NET Core MVC
- Entity Framework Core
- ASP.NET Core Identity
- SQL Server
- Razor Views
- Bootstrap
- jQuery
- Bogus (fake data generation)

---

## Architecture

The project uses a layered architecture with separation of concerns:

### Repositories
Data access abstraction:
- `IRepository<T>`
- `IJobPostingRepository`
- 'IJobApplicationRepository'

### Services
Business logic abstraction:
- `IUserService`
- `IFileService`

### ViewModels
Dedicated ViewModels for forms and UI interactions.

### Entity Relationships
- User ↔ Company (one-to-one)
- Company ↔ JobPosting (one-to-many)
- JobPosting ↔ JobApplication (one-to-many)

---

## Project Structure

```txt
DevSpot/
│
├── Controllers/
├── Data/
│   ├── Seeders/
├── Models/
├── Repositories/
├── Services/
├── ViewModels/
├── Views/
└── Areas/Identity/
```

---

## Getting Started

```bash
# 1. Apply database migrations
dotnet ef database update --project DevSpot

# 2. Run the application
dotnet run --project DevSpot
```

Port is assigned automatically — check the console output after `dotnet run` for the exact URL.

## Demo accounts

After seeding the database you can use ready-made accounts:

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@devsopt.com | Admin123! |
| JobSeeker  | jobseeker@devsopt.com  | JobSeeker123!  |
| Employer  | employer@devsopt.com  | Employer123!  |

These accounts are intended for local development and demo purposes only.

---

## Screenshots
[WIP]

---

## Future Improvements

- Job approval workflow
- Dashboard for employers
- Application management panel
- Email notifications
- Docker support
- REST API
- Unit tests
- Azure/AWS deployment
- Rich text editor for offers

---

## Security Notes

CV uploads are validated by:
- extension
- MIME type
- maximum file size

Authorization policies ensure users can only manage their own resources.

---
