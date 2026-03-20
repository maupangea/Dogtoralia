# Dogtoralia — CLAUDE.md

Veterinary services catalog built with ASP.NET Core 10.0 MVC + EF Core 10.0.5 + SQL Server.

---

## Solution Structure

```
Dogtoralia/
├── DogtoraliaMVC/          # Main web app (net10.0)
│   ├── Controllers/        # ClinicsController, PetsController, AppointmentsController, HomeController
│   ├── Data/               # DogtoraliaDbContext
│   ├── Helpers/            # PaginatedList<T>
│   ├── Migrations/         # EF migrations (auto-generated)
│   ├── Models/             # Speciality, Clinic, Veterinarian, Pet, Appointment, AppointmentStatus
│   ├── ViewModels/         # ClinicsIndexViewModel, ClinicFormViewModel, PetsIndexViewModel, PetFormViewModel, AppointmentsIndexViewModel, AppointmentFormViewModel
│   ├── Views/              # Clinics/, Pets/, Appointments/, Shared/, Home/
│   ├── wwwroot/            # Bootstrap 5, jQuery, site.css
│   ├── appsettings.json    # SQL Server connection string (DefaultConnection)
│   └── Program.cs          # DI config, middleware
└── DogtoraliaMVC.Tests/    # xUnit tests (net10.0)
    ├── Controllers/        # ClinicsControllerTests, PetsControllerTests, AppointmentsControllerTests
    ├── Data/               # DbContextSeedTests
    └── Helpers/            # PaginatedListTests
```

---

## Domain Models

| Model | Key Fields |
|---|---|
| `Speciality` | Id, Name (required, max 100) |
| `Clinic` | Id, Name, Address, Phone, Email, Website?, Description?, CreatedAt, SpecialityId (FK) |
| `Veterinarian` | Id, FirstName, LastName, LicenseNumber (unique), Email, Phone, YearsOfExperience, ClinicId (FK). Computed: `FullName` |
| `Pet` | Id, Name, Species, Breed, DateOfBirth, OwnerName, OwnerEmail, OwnerPhone, Notes?, CreatedAt. Computed: `Age` |
| `Appointment` | Id, ClinicId (FK), PetId (FK), VeterinarianId? (FK), AppointmentDate, Reason (max 500), Notes? (max 1000), Status (enum), CreatedAt |
| `AppointmentStatus` | Enum: Pending=0, Confirmed=1, Cancelled=2, Completed=3 |

**Cascade rules:**
- `Clinic → Veterinarians`: `CASCADE DELETE`
- `Speciality → Clinics`: `RESTRICT` (cannot delete a speciality that has clinics)
- `Clinic → Appointments`: `CASCADE DELETE`
- `Pet → Appointments`: `CASCADE DELETE`
- `Veterinarian → Appointments`: `RESTRICT` (avoid multi-cascade-path SQL Server error)

---

## Database & Migrations

Connection string is in `appsettings.json → ConnectionStrings:DefaultConnection`.

```bash
# From DogtoraliaMVC/
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

Seed data is loaded via `HasData` in `DogtoraliaDbContext.OnModelCreating` — it runs as part of migrations, not at app startup. Seed includes:
- 5 Specialities, 6 Clinics, 10 Veterinarians, 10 Pets

**Important:** Never use `DateTime.UtcNow` in `HasData` seeds — EF requires compile-time constants. Use `new DateTime(year, month, day)` literals.

---

## Features

### Clinics (`/Clinics`)
- Bootstrap card grid (`row-cols-1 row-cols-md-3`)
- Filter by speciality via GET query param `specialityId`
- Pagination: page size 6, `PaginatedList<Clinic>`
- Full CRUD: Index, Details, Create, Edit, Delete
- Includes `Speciality` and `Veterinarians` in queries

### Pets (`/Pets`)
- Bootstrap striped table
- Filter by species via GET query param `species`
- Pagination: page size 8, `PaginatedList<Pet>`
- Full CRUD: Index, Details, Create, Edit, Delete
- Species options: Dog, Cat, Bird, Rabbit, Hamster, Other

### Appointments (`/Appointments`)
- Bootstrap striped table
- Filter by clinicId and status via GET query params
- Pagination: page size 8, `PaginatedList<Appointment>`
- Full CRUD: Index, Details, Create, Edit, Delete
- Create pre-filters veterinarians to selected clinic
- Create POST redirects to Clinic Details; Edit POST redirects to Index
- Clinics/Details page shows appointment count + "Ver citas" / "+ Nueva cita" buttons

### Shared
- `_PaginationPartial.cshtml` — Bootstrap 5 `<nav>`, reads `ViewBag.CurrentPage`, `ViewBag.TotalPages`, `ViewBag.RouteValues`
- `_Layout.cshtml` — navbar includes Home, Clinics, Mascotas, Citas, Privacy

---

## ViewModels

| ViewModel | Purpose |
|---|---|
| `ClinicsIndexViewModel` | Index page: paginated clinics + speciality filter list |
| `ClinicFormViewModel` | Create/Edit form with `SelectList SpecialityOptions` |
| `PetsIndexViewModel` | Index page: paginated pets + species filter list |
| `PetFormViewModel` | Create/Edit form with static `SelectList SpeciesOptions` |
| `AppointmentsIndexViewModel` | Index page: paginated appointments + clinic/status filter lists |
| `AppointmentFormViewModel` | Create/Edit form with SelectLists for Clinic, Pet, Veterinarian, Status |

Manual mapping is used (no AutoMapper) — controllers map ViewModel ↔ entity explicitly.

---

## Testing

```bash
# From solution root or DogtoraliaMVC.Tests/
dotnet test
```

- **Strategy:** xUnit + EF Core InMemory provider. Each test creates an isolated in-memory DB with a unique `Guid` name.
- **Seed in tests:** `ctx.Database.EnsureCreated()` triggers `HasData` seeds automatically.
- **60 tests total:** controller CRUD (Create/Read/Update/Delete GET+POST), pagination helper, and seed data integrity.

---

## Language Policy

All UI text visible in the web interface (Views) must be in **Spanish**. This includes labels, headings, buttons, alerts, table headers, placeholder options, and navigation links.

The backend (controllers, models, viewmodels, DbContext, migrations, tests) remains in **English**.

---

## Common Commands

```bash
# Build
dotnet build

# Run app
dotnet run --project DogtoraliaMVC

# Run tests
dotnet test

# Add migration
dotnet ef migrations add <Name> --project DogtoraliaMVC

# Apply migration
dotnet ef database update --project DogtoraliaMVC
```
