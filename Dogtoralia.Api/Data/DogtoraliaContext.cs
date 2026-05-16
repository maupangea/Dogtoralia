using Dogtoralia.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Dogtoralia.Api.Data;

public class DogtoraliaContext : DbContext
{
    public DogtoraliaContext(DbContextOptions<DogtoraliaContext> options) : base(options) { }

    public DbSet<Speciality> Specialities => Set<Speciality>();
    public DbSet<Clinic> Clinics => Set<Clinic>();
    public DbSet<Veterinarian> Veterinarians => Set<Veterinarian>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<PetOwner> PetOwners => Set<PetOwner>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Veterinarian>()
            .HasIndex(v => v.LicenseNumber)
            .IsUnique();

        modelBuilder.Entity<Veterinarian>()
            .HasOne(v => v.Clinic)
            .WithMany(c => c.Veterinarians)
            .HasForeignKey(v => v.ClinicId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Clinic>()
            .HasOne(c => c.Speciality)
            .WithMany(s => s.Clinics)
            .HasForeignKey(c => c.SpecialityId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Clinic).WithMany(c => c.Appointments)
            .HasForeignKey(a => a.ClinicId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Pet).WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PetId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Veterinarian).WithMany(v => v.Appointments)
            .HasForeignKey(a => a.VeterinarianId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PetOwner>()
            .HasIndex(o => o.Email)
            .IsUnique();

        modelBuilder.Entity<PetOwner>()
            .HasIndex(o => o.UserId)
            .IsUnique()
            .HasFilter("[UserId] IS NOT NULL");

        modelBuilder.Entity<Pet>()
            .HasOne(p => p.PetOwner)
            .WithMany(o => o.Pets)
            .HasForeignKey(p => p.PetOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed data (used by EnsureCreated in tests)
        modelBuilder.Entity<Speciality>().HasData(
            new Speciality { Id = 1, Name = "Medicina General" },
            new Speciality { Id = 2, Name = "Dermatología" },
            new Speciality { Id = 3, Name = "Ortopedia" },
            new Speciality { Id = 4, Name = "Cardiología" },
            new Speciality { Id = 5, Name = "Oncología" }
        );

        modelBuilder.Entity<Clinic>().HasData(
            new Clinic { Id = 1, Name = "Clínica Veterinaria San Ángel", SpecialityId = 1, Address = "Av. Insurgentes Sur 1234, San Ángel, CDMX", Phone = "+52-55-5550-1001", Email = "contacto@sanangelvет.mx", Website = "https://sanangelvет.mx", Description = "Clínica de medicina general para mascotas en San Ángel.", CreatedAt = new DateTime(2024, 1, 15) },
            new Clinic { Id = 2, Name = "DermaPet Polanco", SpecialityId = 2, Address = "Presidente Masaryk 321, Polanco, CDMX", Phone = "+52-55-5550-2002", Email = "info@dermapetpolanco.mx", Website = "https://dermapetpolanco.mx", Description = "Especialistas en dermatología veterinaria.", CreatedAt = new DateTime(2024, 2, 10) },
            new Clinic { Id = 3, Name = "OrtoPaws Guadalajara", SpecialityId = 3, Address = "Av. Vallarta 4560, Guadalajara, Jalisco", Phone = "+52-33-3333-3003", Email = "ortopaws@gdl.mx", Website = null, Description = "Cirugía y rehabilitación ortopédica para animales.", CreatedAt = new DateTime(2024, 3, 5) },
            new Clinic { Id = 4, Name = "CardioVet Monterrey", SpecialityId = 4, Address = "Av. Garza Sada 2501, Monterrey, NL", Phone = "+52-81-8181-4004", Email = "cardiovet@mty.mx", Website = "https://cardiovetmty.mx", Description = "Cardiología veterinaria de alta especialidad.", CreatedAt = new DateTime(2024, 4, 20) },
            new Clinic { Id = 5, Name = "OncoAnimal Puebla", SpecialityId = 5, Address = "Blvd. Atlixcáyotl 2000, Puebla, PUE", Phone = "+52-22-2222-5005", Email = "oncoanimal@puebla.mx", Website = null, Description = "Centro oncológico veterinario en Puebla.", CreatedAt = new DateTime(2024, 5, 12) },
            new Clinic { Id = 6, Name = "Clínica Integral Coyoacán", SpecialityId = 1, Address = "Francisco Sosa 150, Coyoacán, CDMX", Phone = "+52-55-5550-6006", Email = "integral@coyoacanvet.mx", Website = "https://coyoacanvet.mx", Description = "Atención integral y medicina preventiva para mascotas.", CreatedAt = new DateTime(2024, 6, 1) }
        );

        modelBuilder.Entity<Veterinarian>().HasData(
            new Veterinarian { Id = 1, FirstName = "Carlos", LastName = "Mendoza Ruiz", LicenseNumber = "MV-100001", Email = "c.mendoza@sanangelvет.mx", Phone = "+52-55-5550-1101", YearsOfExperience = 10, ClinicId = 1 },
            new Veterinarian { Id = 2, FirstName = "Laura", LastName = "García Pérez", LicenseNumber = "MV-100002", Email = "l.garcia@sanangelvет.mx", Phone = "+52-55-5550-1102", YearsOfExperience = 7, ClinicId = 1 },
            new Veterinarian { Id = 3, FirstName = "Miguel", LastName = "Torres Sánchez", LicenseNumber = "MV-100003", Email = "m.torres@dermapetpolanco.mx", Phone = "+52-55-5550-2101", YearsOfExperience = 12, ClinicId = 2 },
            new Veterinarian { Id = 4, FirstName = "Ana", LastName = "López Hernández", LicenseNumber = "MV-100004", Email = "a.lopez@dermapetpolanco.mx", Phone = "+52-55-5550-2102", YearsOfExperience = 5, ClinicId = 2 },
            new Veterinarian { Id = 5, FirstName = "Roberto", LastName = "Castillo Vega", LicenseNumber = "MV-100005", Email = "r.castillo@gdl.mx", Phone = "+52-33-3333-3101", YearsOfExperience = 15, ClinicId = 3 },
            new Veterinarian { Id = 6, FirstName = "Sofia", LastName = "Ramírez Díaz", LicenseNumber = "MV-100006", Email = "s.ramirez@gdl.mx", Phone = "+52-33-3333-3102", YearsOfExperience = 8, ClinicId = 3 },
            new Veterinarian { Id = 7, FirstName = "Diego", LastName = "Morales Cruz", LicenseNumber = "MV-100007", Email = "d.morales@mty.mx", Phone = "+52-81-8181-4101", YearsOfExperience = 20, ClinicId = 4 },
            new Veterinarian { Id = 8, FirstName = "Valeria", LastName = "Jiménez Flores", LicenseNumber = "MV-100008", Email = "v.jimenez@mty.mx", Phone = "+52-81-8181-4102", YearsOfExperience = 6, ClinicId = 4 },
            new Veterinarian { Id = 9, FirstName = "Héctor", LastName = "Reyes Martínez", LicenseNumber = "MV-100009", Email = "h.reyes@puebla.mx", Phone = "+52-22-2222-5101", YearsOfExperience = 18, ClinicId = 5 },
            new Veterinarian { Id = 10, FirstName = "Carmen", LastName = "Navarro Ortiz", LicenseNumber = "MV-100010", Email = "c.navarro@coyoacanvet.mx", Phone = "+52-55-5550-6101", YearsOfExperience = 9, ClinicId = 6 }
        );

        modelBuilder.Entity<PetOwner>().HasData(
            new PetOwner { Id = 1, Name = "Jorge Sánchez", Email = "jorge.sanchez@gmail.com", Phone = "+52-55-1234-5601", CreatedAt = new DateTime(2024, 1, 20) },
            new PetOwner { Id = 2, Name = "María Fernández", Email = "maria.fernandez@outlook.com", Phone = "+52-55-1234-5602", CreatedAt = new DateTime(2024, 2, 5) },
            new PetOwner { Id = 3, Name = "Andrés Gómez", Email = "andres.gomez@yahoo.com", Phone = "+52-33-9876-5603", CreatedAt = new DateTime(2024, 2, 18) },
            new PetOwner { Id = 4, Name = "Patricia López", Email = "patricia.lopez@gmail.com", Phone = "+52-81-5555-5604", CreatedAt = new DateTime(2024, 3, 10) },
            new PetOwner { Id = 5, Name = "Roberto Díaz", Email = "roberto.diaz@hotmail.com", Phone = "+52-55-1234-5605", CreatedAt = new DateTime(2024, 3, 22) },
            new PetOwner { Id = 6, Name = "Lucía Herrera", Email = "lucia.herrera@gmail.com", Phone = "+52-22-7777-5606", CreatedAt = new DateTime(2024, 4, 1) },
            new PetOwner { Id = 7, Name = "Eduardo Torres", Email = "eduardo.torres@gmail.com", Phone = "+52-55-1234-5607", CreatedAt = new DateTime(2024, 4, 15) },
            new PetOwner { Id = 8, Name = "Gabriela Ruiz", Email = "gabriela.ruiz@outlook.com", Phone = "+52-33-3333-5608", CreatedAt = new DateTime(2024, 5, 5) },
            new PetOwner { Id = 9, Name = "Felipe Morales", Email = "felipe.morales@yahoo.com", Phone = "+52-81-8888-5609", CreatedAt = new DateTime(2024, 5, 20) },
            new PetOwner { Id = 10, Name = "Isabella Castro", Email = "isabella.castro@gmail.com", Phone = "+52-55-1234-5610", CreatedAt = new DateTime(2024, 6, 10) }
        );

        modelBuilder.Entity<Pet>().HasData(
            new Pet { Id = 1, Name = "Max", Species = "Perro", Breed = "Labrador Retriever", DateOfBirth = new DateTime(2020, 3, 15), Notes = "Alérgico al pollo.", CreatedAt = new DateTime(2024, 1, 20), PetOwnerId = 1 },
            new Pet { Id = 2, Name = "Luna", Species = "Gato", Breed = "Siamese", DateOfBirth = new DateTime(2021, 7, 22), Notes = null, CreatedAt = new DateTime(2024, 2, 5), PetOwnerId = 2 },
            new Pet { Id = 3, Name = "Paco", Species = "Ave", Breed = "African Grey Parrot", DateOfBirth = new DateTime(2018, 11, 10), Notes = "Habla español.", CreatedAt = new DateTime(2024, 2, 18), PetOwnerId = 3 },
            new Pet { Id = 4, Name = "Bella", Species = "Perro", Breed = "Golden Retriever", DateOfBirth = new DateTime(2019, 5, 30), Notes = null, CreatedAt = new DateTime(2024, 3, 10), PetOwnerId = 4 },
            new Pet { Id = 5, Name = "Mimi", Species = "Gato", Breed = "Persian", DateOfBirth = new DateTime(2022, 1, 14), Notes = "Pelo largo, requiere grooming mensual.", CreatedAt = new DateTime(2024, 3, 22), PetOwnerId = 5 },
            new Pet { Id = 6, Name = "Rocky", Species = "Perro", Breed = "German Shepherd", DateOfBirth = new DateTime(2020, 9, 8), Notes = null, CreatedAt = new DateTime(2024, 4, 1), PetOwnerId = 6 },
            new Pet { Id = 7, Name = "Coco", Species = "Ave", Breed = "Cockatiel", DateOfBirth = new DateTime(2023, 2, 28), Notes = "Muy sociable.", CreatedAt = new DateTime(2024, 4, 15), PetOwnerId = 7 },
            new Pet { Id = 8, Name = "Nala", Species = "Gato", Breed = "Maine Coon", DateOfBirth = new DateTime(2020, 6, 3), Notes = null, CreatedAt = new DateTime(2024, 5, 5), PetOwnerId = 8 },
            new Pet { Id = 9, Name = "Tito", Species = "Conejo", Breed = "Holland Lop", DateOfBirth = new DateTime(2022, 10, 17), Notes = "Dieta especial sin pellets.", CreatedAt = new DateTime(2024, 5, 20), PetOwnerId = 9 },
            new Pet { Id = 10, Name = "Kira", Species = "Perro", Breed = "Beagle", DateOfBirth = new DateTime(2021, 12, 25), Notes = null, CreatedAt = new DateTime(2024, 6, 10), PetOwnerId = 10 }
        );
    }
}
