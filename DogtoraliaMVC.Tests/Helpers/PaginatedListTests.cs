using DogtoraliaMVC.Data;
using DogtoraliaMVC.Helpers;
using DogtoraliaMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Tests.Helpers;

public class PaginatedListTests
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

    private static void AddExtraPets(DogtoraliaDbContext ctx, int count)
    {
        for (int i = 0; i < count; i++)
            ctx.Pets.Add(new Pet
            {
                Name = $"TestPet{i}",
                Species = "Gato",
                Breed = "Test",
                DateOfBirth = new DateTime(2021, 1, 1),
                PetOwnerId = 1,
                CreatedAt = DateTime.UtcNow
            });
        ctx.SaveChanges();
    }

    [Fact]
    public async Task FirstPage_ReturnsCorrectItems()
    {
        using var ctx = CreateContext();
        // 10 seeded pets; page 1, size 5 → 5 items, 2 total pages
        var query = ctx.Pets.OrderBy(p => p.Name).AsQueryable();
        var list = await PaginatedList<Pet>.CreateAsync(query, pageIndex: 1, pageSize: 5);

        Assert.Equal(1, list.PageIndex);
        Assert.Equal(2, list.TotalPages);
        Assert.Equal(5, list.Count);
        Assert.False(list.HasPreviousPage);
        Assert.True(list.HasNextPage);
    }

    [Fact]
    public async Task LastPage_ReturnsRemainder()
    {
        using var ctx = CreateContext();
        // 10 seeded + 2 extra = 12 pets; page 2, size 10 → 2 items on page 2
        AddExtraPets(ctx, 2);
        var query = ctx.Pets.OrderBy(p => p.Name).AsQueryable();
        var list = await PaginatedList<Pet>.CreateAsync(query, pageIndex: 2, pageSize: 10);

        Assert.Equal(2, list.PageIndex);
        Assert.Equal(2, list.TotalPages);
        Assert.Equal(2, list.Count);
        Assert.True(list.HasPreviousPage);
        Assert.False(list.HasNextPage);
    }

    [Fact]
    public async Task SinglePage_NoPreviousOrNext()
    {
        using var ctx = CreateContext();
        var query = ctx.Pets.Where(p => p.Species == "Conejo").OrderBy(p => p.Name).AsQueryable();
        var list = await PaginatedList<Pet>.CreateAsync(query, pageIndex: 1, pageSize: 10);

        Assert.Equal(1, list.TotalPages);
        Assert.False(list.HasPreviousPage);
        Assert.False(list.HasNextPage);
    }

    [Fact]
    public async Task TotalPages_RoundsUp()
    {
        using var ctx = CreateContext();
        // 10 seeded + 1 extra = 11 pets; page size 5 → ceil(11/5) = 3 total pages
        AddExtraPets(ctx, 1);
        var query = ctx.Pets.OrderBy(p => p.Name).AsQueryable();
        var list = await PaginatedList<Pet>.CreateAsync(query, pageIndex: 1, pageSize: 5);

        Assert.Equal(3, list.TotalPages);
    }
}
