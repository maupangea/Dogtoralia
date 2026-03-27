# Dogtoralia

Veterinary services catalog built with **ASP.NET Core 10.0 MVC**, **EF Core 10.0.5**, and **SQL Server**. Aditional documentation and diagrams can be found in `Docs/` directory.

---

## Features

- **Clinics** — Browse and manage veterinary clinics, filter by speciality, paginated card grid
- **Pet Owners** — Manage pet owners; each owner has a profile page listing their pets
- **Pets** — Register and manage pets, each linked to a pet owner, filter by species
- **Appointments** — Schedule appointments between pets, clinics, and veterinarians, filter by clinic and status
- **Veterinarians** — Managed as part of each clinic

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 10.0 MVC |
| ORM | Entity Framework Core 10.0.5 |
| Database | SQL Server |
| Frontend | Bootstrap 5, jQuery |
| Testing | xUnit + EF Core InMemory |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or remote)
- EF Core CLI: `dotnet tool install --global dotnet-ef`

### Setup

1. Clone the repository:
   ```bash
   git clone <repo-url>
   cd Dogtoralia
   ```

2. Update the connection string in `DogtoraliaMVC/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=Dogtoralia;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

3. Apply migrations and seed data:
   ```bash
   dotnet ef database update --project DogtoraliaMVC
   ```

4. Run the app:
   ```bash
   dotnet run --project DogtoraliaMVC
   ```

5. Open `https://localhost:<port>` in your browser.

---

## Running Tests

```bash
dotnet test
```

81 tests covering controller CRUD, seed data integrity, and pagination.

---

## Project Structure

```
Dogtoralia/
├── DogtoraliaMVC/          # Main web application
│   ├── Controllers/
│   ├── Data/               # DogtoraliaDbContext
│   ├── Helpers/            # PaginatedList<T>
│   ├── Migrations/
│   ├── Models/
│   ├── ViewModels/
│   └── Views/
└── DogtoraliaMVC.Tests/    # xUnit test project
    ├── Controllers/
    ├── Data/
    └── Helpers/
```

---

## Domain Model

```
PetOwner ──< Pet ──< Appointment >── Clinic ──< Veterinarian
                                        │
                                    Speciality
```

- A **PetOwner** has many **Pets**
- A **Pet** belongs to one **PetOwner** and can have many **Appointments**
- An **Appointment** links a **Pet** to a **Clinic** and optionally a **Veterinarian**
- A **Clinic** belongs to one **Speciality** and has many **Veterinarians**
