# Dogtoralia — CLAUDE.md

Veterinary services catalog built with ASP.NET Core 10.0 MVC + EF Core 10.0.5 + SQL Server.

---

## Solution Structure

```
Dogtoralia/
├── DogtoraliaMVC/          # Main web app (net10.0)
│   ├── Controllers/        # ClinicsController, PetsController, AppointmentsController, PetOwnersController, HomeController
│   ├── Data/               # DogtoraliaDbContext
│   ├── Helpers/            # PaginatedList<T>
│   ├── Migrations/         # EF migrations (auto-generated)
│   ├── Models/             # Speciality, Clinic, Veterinarian, Pet, PetOwner, Appointment, AppointmentStatus
│   ├── ViewModels/         # ClinicsIndexViewModel, ClinicFormViewModel, PetsIndexViewModel, PetFormViewModel, AppointmentsIndexViewModel, AppointmentFormViewModel, PetOwnerFormViewModel, PetOwnerDetailsViewModel
│   ├── Views/              # Clinics/, Pets/, Appointments/, PetOwners/, Shared/, Home/
│   ├── wwwroot/            # Bootstrap 5, jQuery, site.css
│   ├── appsettings.json    # SQL Server connection string (DefaultConnection)
│   └── Program.cs          # DI config, middleware
└── DogtoraliaMVC.Tests/    # xUnit tests (net10.0)
    ├── Controllers/        # ClinicsControllerTests, PetsControllerTests, AppointmentsControllerTests, PetOwnersControllerTests
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
| `PetOwner` | Id, Name (required, max 200), Email (required, unique, max 200), Phone (required, max 20), CreatedAt |
| `Pet` | Id, Name, Species, Breed, DateOfBirth, Notes?, CreatedAt, PetOwnerId (FK, required). Computed: `Age` |
| `Appointment` | Id, ClinicId (FK), PetId (FK), VeterinarianId? (FK), AppointmentDate, Reason (max 500), Notes? (max 1000), Status (enum), CreatedAt |
| `AppointmentStatus` | Enum: Pending=0, Confirmed=1, Cancelled=2, Completed=3 |

**Cascade rules:**
- `Clinic → Veterinarians`: `CASCADE DELETE`
- `Speciality → Clinics`: `RESTRICT` (cannot delete a speciality that has clinics)
- `Clinic → Appointments`: `CASCADE DELETE`
- `Pet → Appointments`: `CASCADE DELETE`
- `Veterinarian → Appointments`: `RESTRICT` (avoid multi-cascade-path SQL Server error)
- `PetOwner → Pets`: `RESTRICT` (cannot delete an owner who has pets)

---

## Database & Migrations

Connection string is in `appsettings.json → ConnectionStrings:DefaultConnection`.

```bash
# From DogtoraliaMVC/
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

Seed data is loaded via `HasData` in `DogtoraliaDbContext.OnModelCreating` — it runs as part of migrations, not at app startup. Seed includes:
- 5 Specialities, 6 Clinics, 10 Veterinarians, 10 PetOwners, 10 Pets (each Pet linked to a PetOwner)

**Important:** Never use `DateTime.UtcNow` in `HasData` seeds — EF requires compile-time constants. Use `new DateTime(year, month, day)` literals.

---

## Features

### Clinics (`/Clinics`)
- Bootstrap card grid (`row-cols-1 row-cols-md-3`)
- Filter by speciality via GET query param `specialityId`
- Pagination: page size 6, `PaginatedList<Clinic>`
- Full CRUD: Index, Details, Create, Edit, Delete
- Includes `Speciality` and `Veterinarians` in queries

### Pet Owners (`/PetOwners`)
- Bootstrap striped table listing all owners with pet count
- Full CRUD: Index, Details, Create, Edit, Delete
- Details page shows the owner's pets with Edit/Delete/View actions
- Delete is blocked in the UI (and restricted in DB) if the owner has pets
- Email uniqueness validated on Create and Edit

### Pets (`/Pets`)
- Bootstrap striped table; owner column links to `PetOwners/Details`
- Filter by species via GET query param `species`
- Pagination: page size 8, `PaginatedList<Pet>`
- **Pets are created from `PetOwners/Details`** — the Create form is pre-filled with owner info (read-only) and accepts a `petOwnerId` route param
- Create/Edit/Delete all redirect back to `PetOwners/Details` after saving
- Species options: Perro, Gato, Ave, Conejo, Hámster, Otro
- `Pet.PetOwnerId` is required (non-nullable FK); owner info is read via navigation property — no duplicate owner fields on Pet

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
- `_Layout.cshtml` — navbar includes Inicio, Clínicas, Propietarios, Mascotas, Citas, Privacidad

---

## ViewModels

| ViewModel | Purpose |
|---|---|
| `ClinicsIndexViewModel` | Index page: paginated clinics + speciality filter list |
| `ClinicFormViewModel` | Create/Edit form with `SelectList SpecialityOptions` |
| `PetsIndexViewModel` | Index page: paginated pets + species filter list |
| `PetFormViewModel` | Create/Edit form — holds `PetOwnerId` (required) + display-only owner fields + `SelectList SpeciesOptions` |
| `AppointmentsIndexViewModel` | Index page: paginated appointments + clinic/status filter lists |
| `AppointmentFormViewModel` | Create/Edit form with SelectLists for Clinic, Pet, Veterinarian, Status |
| `PetOwnerFormViewModel` | Create/Edit form with Name, Email, Phone validation |
| `PetOwnerDetailsViewModel` | Details page: `PetOwner Owner` + `List<Pet> Pets` |

Manual mapping is used (no AutoMapper) — controllers map ViewModel ↔ entity explicitly.

---

## Testing

```bash
# From solution root or DogtoraliaMVC.Tests/
dotnet test
```

- **Strategy:** xUnit + EF Core InMemory provider. Each test creates an isolated in-memory DB with a unique `Guid` name.
- **Seed in tests:** `ctx.Database.EnsureCreated()` triggers `HasData` seeds automatically.
- **81 tests total:** controller CRUD (Create/Read/Update/Delete GET+POST), pagination helper, and seed data integrity.

---

## Language Policy

All UI text visible in the web interface (Views) must be in **Spanish**. This includes labels, headings, buttons, alerts, table headers, placeholder options, and navigation links.

The backend (controllers, models, viewmodels, DbContext, migrations, tests) remains in **English**.

This policy also applies to the MAUI app: page text (`Dogtoralia.Maui/Views/**`) and user-facing `ViewModel` strings (titles, error messages) are in **Spanish**; service/ViewModel logic stays in **English**.

---

## .NET MAUI App

A cross-platform client (`Dogtoralia.Maui`) consumes `Dogtoralia.Api` over HTTP. It follows MVVM, with all testable logic isolated in a plain `net10.0` library so it can be unit-tested without a device/simulator.

```
Dogtoralia.Maui/             # MAUI head project (net10.0-android/ios/maccatalyst/windows)
├── ApiConfig.cs             # Platform-aware API base URL
├── AppShell.xaml(.cs)       # Flyout nav (Inicio, Clínicas, Mascotas, Veterinarios) + route registration
├── MauiProgram.cs           # DI: HttpClient + I*ApiService + ViewModels + Pages
├── Views/                   # HomePage, ClinicsPage, ClinicDetailPage, VeterinariansPage,
│                            #   VeterinarianDetailPage, PetsPage, PetDetailPage, PetEditPage
└── Platforms/               # Android manifest (cleartext) + iOS/MacCatalyst Info.plist (ATS localhost)

Dogtoralia.Maui.Core/        # net10.0 library — referenced by the app AND Dogtoralia.Tests
├── Services/                # I*ApiService + impls over an injected HttpClient (System.Net.Http.Json)
│                            #   Clinic (read), Veterinarian (read), Pet (full CRUD), PetOwner (read)
└── ViewModels/              # BaseViewModel + Clinics/ClinicDetail/Veterinarians/VeterinarianDetail/
                             #   Pets/PetDetail/PetEdit (CommunityToolkit.Mvvm)
```

**Key points:**
- **DTOs are reused from `Dogtoralia.Shared/Dtos`** — no duplicate MAUI models. The API has no pagination, so services return full `IReadOnlyList<T>`.
- **API base URL** (`ApiConfig.BaseUrl`): Android emulator → `http://10.0.2.2:5186`; iOS/Mac Catalyst/Windows → `http://localhost:5186`. Cleartext HTTP is enabled for local dev.
- **Navigation:** Shell routes; detail/edit pages receive `id` via `[QueryProperty]` and load in `OnAppearing`. The Pet form's owner picker is filled from `/api/petowners`; species is the same fixed list as MVC.
- **No auth / no Appointments** in the mobile app (out of scope).
- **Tests:** `Dogtoralia.Tests/Maui/` — ViewModel tests mock the `I*ApiService` interfaces (Moq); service tests use a stub `HttpMessageHandler` to assert URL/verb and deserialization.

> `Dogtoralia.Maui.Core` and the test suite build/run on any net10.0 SDK without a platform toolchain. The MAUI **head** project needs the Apple/Android toolchain and carries two macOS workarounds in its csproj (both overridable):
> - `ValidateXcodeVersion=false` — the released MAUI workload (`10.0.204.1`) pins the Apple SDK to Xcode 26.4, but the dev machine has Xcode 26.5; the one-minor delta is safe locally. Remove once a workload targeting 26.5 ships.
> - `BaseOutputPath=$(HOME)/.dogtoralia-build/...` on macOS — the repo lives under iCloud-synced `~/Documents`, which stamps `com.apple.FinderInfo` xattrs that make `codesign` reject the `.app`. Building `bin/` outside iCloud avoids it. The permanent fix is to move the repo out of `~/Documents`.

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

# Build the MAUI core library (no platform toolchain needed)
dotnet build Dogtoralia.Maui.Core

# Run the MAUI app on Mac Catalyst (start Dogtoralia.Api first)
dotnet build Dogtoralia.Maui -t:Run -f net10.0-maccatalyst
```
