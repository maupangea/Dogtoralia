using Dogtoralia.MVC.Data;
using Microsoft.EntityFrameworkCore;

namespace Dogtoralia.Tests.Data;

public class DbContextSeedTests
{
    private static DogtoraliaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DogtoraliaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var ctx = new DogtoraliaDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    [Fact]
    public void Seed_HasFiveSpecialities()
    {
        using var ctx = CreateContext();
        Assert.Equal(5, ctx.Specialities.Count());
    }

    [Fact]
    public void Seed_HasSixClinics()
    {
        using var ctx = CreateContext();
        Assert.Equal(6, ctx.Clinics.Count());
    }

    [Fact]
    public void Seed_HasTenVeterinarians()
    {
        using var ctx = CreateContext();
        Assert.Equal(10, ctx.Veterinarians.Count());
    }

    [Fact]
    public void Seed_HasTenPets()
    {
        using var ctx = CreateContext();
        Assert.Equal(10, ctx.Pets.Count());
    }

    [Fact]
    public void Seed_HasTenPetOwners()
    {
        using var ctx = CreateContext();
        Assert.Equal(10, ctx.PetOwners.Count());
    }

    [Fact]
    public void Seed_AllClinics_HaveValidSpecialityFk()
    {
        using var ctx = CreateContext();
        var specialityIds = ctx.Specialities.Select(s => s.Id).ToHashSet();
        var allValid = ctx.Clinics.All(c => specialityIds.Contains(c.SpecialityId));
        Assert.True(allValid);
    }

    [Fact]
    public void Seed_AllVeterinarians_HaveValidClinicFk()
    {
        using var ctx = CreateContext();
        var clinicIds = ctx.Clinics.Select(c => c.Id).ToHashSet();
        var allValid = ctx.Veterinarians.All(v => clinicIds.Contains(v.ClinicId));
        Assert.True(allValid);
    }

    [Fact]
    public void Seed_VeterinarianLicenseNumbers_AreUnique()
    {
        using var ctx = CreateContext();
        var licenses = ctx.Veterinarians.Select(v => v.LicenseNumber).ToList();
        Assert.Equal(licenses.Count, licenses.Distinct().Count());
    }

    [Fact]
    public void Seed_AllPets_HaveValidPetOwnerFk()
    {
        using var ctx = CreateContext();
        var ownerIds = ctx.PetOwners.Select(o => o.Id).ToHashSet();
        var allValid = ctx.Pets.All(p => ownerIds.Contains(p.PetOwnerId));
        Assert.True(allValid);
    }

    [Fact]
    public void Seed_PetOwnerEmails_AreUnique()
    {
        using var ctx = CreateContext();
        var emails = ctx.PetOwners.Select(o => o.Email).ToList();
        Assert.Equal(emails.Count, emails.Distinct().Count());
    }
}
