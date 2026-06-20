# Dogtoralia

Veterinary services catalog built with **ASP.NET Core 10.0 MVC**, **EF Core 10.0.5**, and **SQL Server**. Aditional documentation and diagrams can be found in `Docs/` directory.

---

## Features

- **Clinics** — Browse and manage veterinary clinics, filter by speciality, paginated card grid
- **Pet Owners** — Manage pet owners; each owner has a profile page listing their pets
- **Pets** — Register and manage pets, each linked to a pet owner, filter by species
- **Appointments** — Schedule appointments between pets, clinics, and veterinarians, filter by clinic and status
- **Veterinarians** — Managed as part of each clinic
- **REST API** — `Dogtoralia.Api` exposes the catalog over HTTP for external clients
- **Mobile app** — `Dogtoralia.Maui` (.NET MAUI) consumes the API to browse clinics and veterinarians and manage pets

---

## Tech Stack

| Layer | Technology |
|---|---|
| Web framework | ASP.NET Core 10.0 MVC |
| REST API | ASP.NET Core 10.0 Web API |
| Mobile | .NET MAUI 10.0 (MVVM, CommunityToolkit.Mvvm) |
| ORM | Entity Framework Core 10.0.5 |
| Database | SQL Server |
| Frontend | Bootstrap 5, jQuery |
| Testing | xUnit + Moq + EF Core InMemory |

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

## Mobile app (.NET MAUI)

The `Dogtoralia.Maui` project is a cross-platform mobile/desktop client that consumes `Dogtoralia.Api`. It uses the MVVM pattern; all testable logic (API services + ViewModels) lives in the `Dogtoralia.Maui.Core` net10.0 library so it can be unit-tested without a device.

### Prerequisites

- .NET MAUI workloads: `dotnet workload install maui`
- A platform toolchain for the target you build (Xcode for iOS/Mac Catalyst, Android SDK + JDK for Android, Windows SDK for Windows).

### Running

1. Start the API first (the app talks to it over HTTP):
   ```bash
   dotnet run --project Dogtoralia.Api
   ```
   The API listens on `http://localhost:5186`.

2. Launch the MAUI app on a target, e.g. Mac Catalyst:
   ```bash
   dotnet build Dogtoralia.Maui -t:Run -f net10.0-maccatalyst
   ```
   (Use `-f net10.0-android`, `-f net10.0-ios`, or `-f net10.0-windows10.0.19041.0` for other targets.)

### macOS build notes

The MAUI head project's csproj carries two macOS-only workarounds (both overridable on the command line):

- **Xcode version:** the released .NET MAUI workload pins the Apple SDK to Xcode 26.4. On a machine with a newer Xcode, the build relaxes the strict check (`ValidateXcodeVersion=false`). Pass `-p:ValidateXcodeVersion=true` to re-enable it, or install the matching Xcode.
- **iCloud + codesign:** building inside an iCloud-synced `~/Documents` folder makes `codesign` reject the app (`com.apple.FinderInfo` "detritus"). The build redirects output to `$(HOME)/.dogtoralia-build/...` to avoid it. The cleaner permanent fix is to keep the repo outside `~/Documents` (e.g. `~/Projects`). Override with `-p:BaseOutputPath=bin/` if your checkout is already outside iCloud.

### API base URL

The base URL is selected per platform in `Dogtoralia.Maui/ApiConfig.cs`:

- **Android emulator** → `http://10.0.2.2:5186` (the emulator's alias for the host's `localhost`)
- **iOS simulator / Mac Catalyst / Windows** → `http://localhost:5186`

Cleartext HTTP is enabled for local development (Android `usesCleartextTraffic`, iOS/Mac Catalyst ATS exception for `localhost`). For a physical device, point `ApiConfig.BaseUrl` at your machine's LAN address and host the API accordingly.

> Authentication is intentionally not wired into the mobile app yet; the API currently requires none.

---

## Running Tests

```bash
dotnet test
```

The xUnit suite covers MVC and API controller CRUD, seed-data integrity, pagination, and the MAUI ViewModels and API services (mocked with Moq).

---

## Project Structure

```
Dogtoralia/
├── Dogtoralia.MVC/          # Server-rendered web application (Razor views)
├── Dogtoralia.Api/          # REST API consumed by the mobile app
├── Dogtoralia.Shared/       # Shared domain models + DTOs
├── Dogtoralia.Maui/         # .NET MAUI app — Views, AppShell, DI wiring, platform config
├── Dogtoralia.Maui.Core/    # net10.0 library — API services + MVVM ViewModels (unit-testable)
└── Dogtoralia.Tests/        # xUnit tests (MVC, API, and MAUI Core)
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
