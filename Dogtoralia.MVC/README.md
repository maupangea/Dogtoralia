# Dogtoralia

Catálogo de servicios veterinarios construido con **ASP.NET Core 10.0 MVC**, **EF Core 10.0.5** y **SQL Server**.

---

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local o Docker)
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

---

## Configuración

Actualiza la cadena de conexión en `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=Dogtoralia;User Id=sa;Password=<tu-password>;TrustServerCertificate=True;"
}
```

---

## Primeros pasos

```bash
# Restaurar dependencias y compilar
dotnet build

# Aplicar migraciones y crear la base de datos
dotnet ef database update --project DogtoraliaMVC

# Ejecutar la aplicación
dotnet run --project DogtoraliaMVC
```

La app estará disponible en `https://localhost:5001` (o el puerto que muestre la consola).

---

## Estructura del proyecto

```
DogtoraliaMVC/
├── Controllers/        # ClinicsController, PetsController, AppointmentsController, HomeController
├── Data/               # DogtoraliaDbContext (EF Core)
├── Helpers/            # PaginatedList<T>
├── Migrations/         # Migraciones de EF Core (auto-generadas)
├── Models/             # Speciality, Clinic, Veterinarian, Pet, Appointment, AppointmentStatus
├── ViewModels/         # ViewModels para formularios e índices
├── Views/              # Vistas Razor (Clinics, Pets, Shared, Home)
├── wwwroot/            # Bootstrap 5, jQuery, CSS estático
├── appsettings.json    # Configuración y cadena de conexión
└── Program.cs          # Configuración de DI y middleware
```

---

## Funcionalidades

### Clínicas (`/Clinics`)
- Grid de tarjetas Bootstrap responsive
- Filtro por especialidad
- Paginación (6 por página)
- CRUD completo

### Mascotas (`/Pets`)
- Tabla con filas alternas Bootstrap
- Filtro por especie (Perro, Gato, Ave, Conejo, Hámster, Otro)
- Paginación (8 por página)
- CRUD completo

### Citas (`/Appointments`)
- Tabla con filas alternas Bootstrap
- Filtro por clínica y estado
- Paginación (8 por página)
- CRUD completo
- Creación de cita vinculada a clínica, mascota y veterinario (opcional)

---

## Modelos de dominio

| Modelo | Campos principales |
|---|---|
| `Speciality` | Id, Name |
| `Clinic` | Id, Name, Address, Phone, Email, Website?, Description?, CreatedAt, SpecialityId |
| `Veterinarian` | Id, FirstName, LastName, LicenseNumber, Email, Phone, YearsOfExperience, ClinicId |
| `Pet` | Id, Name, Species, Breed, DateOfBirth, OwnerName, OwnerEmail, OwnerPhone, Notes?, CreatedAt |
| `Appointment` | Id, ClinicId, PetId, VeterinarianId?, AppointmentDate, Reason, Notes?, Status, CreatedAt |

Los datos semilla (5 especialidades, 6 clínicas, 10 veterinarios, 10 mascotas) se cargan mediante `HasData` en las migraciones.

---

## Migraciones

```bash
# Agregar una nueva migración
dotnet ef migrations add <NombreMigracion> --project DogtoraliaMVC

# Aplicar migraciones pendientes
dotnet ef database update --project DogtoraliaMVC
```

---

## Pruebas

```bash
dotnet test
```

60 pruebas con xUnit y EF Core InMemory. Cubren CRUD de controladores, helper de paginación e integridad de datos semilla.
