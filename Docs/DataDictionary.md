# Diccionario de Datos — Dogtoralia

## Entidades

| Clase | Descripción |
|---|---|
| `Speciality` | Categoría de especialidad médica (p. ej., dermatología, cirugía) que clasifica a las clínicas. |
| `Clinic` | Clínica veterinaria que ofrece servicios, asociada a una especialidad. |
| `Veterinarian` | Profesional veterinario con licencia, empleado en una clínica. |
| `PetOwner` | Persona dueña de una o más mascotas que puede agendar citas. |
| `Pet` | Animal perteneciente a un propietario, sujeto a citas veterinarias. |
| `Appointment` | Visita programada que vincula a una mascota con una clínica, opcionalmente asignada a un veterinario. |
| `AppointmentStatus` | Enumerador que representa el estado del ciclo de vida de una cita. |

---

## Campos

### `Speciality` — Especialidad

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int | PK | Identificador generado automáticamente |
| `Name` | string | Requerido, máx. 100 | Nombre de la especialidad (p. ej., "Cirugía") |

---

### `Clinic` — Clínica

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int | PK | Identificador generado automáticamente |
| `Name` | string | Requerido | Nombre de la clínica |
| `Address` | string | Requerido | Ubicación física |
| `Phone` | string | Requerido | Teléfono de contacto |
| `Email` | string | Requerido | Correo electrónico de contacto |
| `Website` | string | Opcional | Sitio web de la clínica |
| `Description` | string | Opcional | Descripción en texto libre |
| `CreatedAt` | DateTime | Requerido | Fecha y hora de creación del registro |
| `SpecialityId` | int | FK → Speciality | Especialidad médica de la clínica |

---

### `Veterinarian` — Veterinario

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int | PK | Identificador generado automáticamente |
| `FirstName` | string | Requerido | Nombre(s) |
| `LastName` | string | Requerido | Apellido(s) |
| `LicenseNumber` | string | Requerido, único | Número de licencia profesional |
| `Email` | string | Requerido | Correo electrónico de contacto |
| `Phone` | string | Requerido | Teléfono de contacto |
| `YearsOfExperience` | int | Requerido | Años de experiencia en la práctica |
| `ClinicId` | int | FK → Clinic | Clínica empleadora |
| `FullName` *(calculado)* | string | — | `FirstName + " " + LastName` |

---

### `PetOwner` — Propietario de Mascota

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int | PK | Identificador generado automáticamente |
| `Name` | string | Requerido, máx. 200 | Nombre completo del propietario |
| `Email` | string | Requerido, único, máx. 200 | Correo electrónico (debe ser único) |
| `Phone` | string | Requerido, máx. 20 | Teléfono de contacto |
| `CreatedAt` | DateTime | Requerido | Fecha y hora de creación del registro |

---

### `Pet` — Mascota

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int | PK | Identificador generado automáticamente |
| `Name` | string | Requerido | Nombre de la mascota |
| `Species` | string | Requerido | Especie (Perro, Gato, Ave, Conejo, Hámster, Otro) |
| `Breed` | string | Requerido | Raza o variedad |
| `DateOfBirth` | DateTime | Requerido | Fecha de nacimiento de la mascota |
| `Notes` | string | Opcional | Notas adicionales de salud o cuidado |
| `CreatedAt` | DateTime | Requerido | Fecha y hora de creación del registro |
| `PetOwnerId` | int | FK → PetOwner | Propietario de la mascota |
| `Age` *(calculado)* | int | — | Derivado de `DateOfBirth` |

---

### `Appointment` — Cita

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int | PK | Identificador generado automáticamente |
| `ClinicId` | int | FK → Clinic | Clínica que atiende la cita |
| `PetId` | int | FK → Pet | Mascota que asiste a la cita |
| `VeterinarianId` | int? | FK → Veterinarian, opcional | Veterinario asignado (puede ser nulo) |
| `AppointmentDate` | DateTime | Requerido | Fecha y hora programada |
| `Reason` | string | Requerido, máx. 500 | Motivo de la visita |
| `Notes` | string | Opcional, máx. 1000 | Notas posteriores a la visita o adicionales |
| `Status` | AppointmentStatus | Requerido | Estado actual del ciclo de vida |
| `CreatedAt` | DateTime | Requerido | Fecha y hora de creación del registro |

---

### `AppointmentStatus` — Estado de Cita *(enumerador)*

| Valor | Entero | Descripción |
|---|---|---|
| `Pending` | 0 | Cita solicitada, en espera de confirmación |
| `Confirmed` | 1 | Cita confirmada por la clínica |
| `Cancelled` | 2 | Cita cancelada |
| `Completed` | 3 | Cita realizada |

---

## Reglas de Cascada y Restricción

| Relación | Regla | Justificación |
|---|---|---|
| Speciality → Clinics | RESTRICT | No se puede eliminar una especialidad que tenga clínicas activas |
| Clinic → Veterinarians | CASCADE DELETE | Los veterinarios pertenecen a la clínica; se eliminan junto con ella |
| Clinic → Appointments | CASCADE DELETE | Las citas están ligadas al ciclo de vida de la clínica |
| PetOwner → Pets | RESTRICT | No se puede eliminar un propietario que aún tenga mascotas |
| Pet → Appointments | CASCADE DELETE | Las citas se eliminan junto con la mascota |
| Veterinarian → Appointments | RESTRICT | Evita el error de rutas de cascada múltiples en SQL Server |
