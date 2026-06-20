using System.Net;
using Dogtoralia.Maui.Core.Services;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Tests.Maui.Services;

public class PetApiServiceTests
{
    private static HttpClient CreateClient(MockHttpMessageHandler handler) =>
        new(handler) { BaseAddress = new Uri("http://localhost:5186") };

    [Fact]
    public async Task GetAllAsync_ReturnsDeserializedPets()
    {
        const string json = """
        [
          { "id": 1, "name": "Rex", "species": "Perro", "petOwnerId": 1 },
          { "id": 2, "name": "Mia", "species": "Gato", "petOwnerId": 2 }
        ]
        """;
        var handler = new MockHttpMessageHandler(HttpStatusCode.OK, json);
        var service = new PetApiService(CreateClient(handler));

        var pets = await service.GetAllAsync();

        Assert.Equal(2, pets.Count);
        Assert.Equal("Rex", pets[0].Name);
        Assert.Equal(HttpMethod.Get, handler.LastRequest!.Method);
        Assert.Equal("/api/pets", handler.LastRequest.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task GetByIdAsync_RequestsCorrectUrl()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.OK,
            """{ "id": 7, "name": "Rex", "species": "Perro", "petOwnerId": 1 }""");
        var service = new PetApiService(CreateClient(handler));

        var pet = await service.GetByIdAsync(7);

        Assert.NotNull(pet);
        Assert.Equal(7, pet!.Id);
        Assert.Equal("/api/pets/7", handler.LastRequest!.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task CreateAsync_PostsToPetsAndReturnsCreated()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.Created,
            """{ "id": 50, "name": "Bobby", "species": "Gato", "petOwnerId": 3 }""");
        var service = new PetApiService(CreateClient(handler));

        var created = await service.CreateAsync(new PetWriteDto
        {
            Name = "Bobby", Species = "Gato", DateOfBirth = new DateTime(2021, 1, 1), PetOwnerId = 3
        });

        Assert.NotNull(created);
        Assert.Equal(50, created!.Id);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("/api/pets", handler.LastRequest.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task UpdateAsync_SendsPutToCorrectUrl()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.NoContent);
        var service = new PetApiService(CreateClient(handler));

        await service.UpdateAsync(9, new PetWriteDto
        {
            Name = "Up", Species = "Perro", DateOfBirth = new DateTime(2020, 1, 1), PetOwnerId = 1
        });

        Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
        Assert.Equal("/api/pets/9", handler.LastRequest.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task DeleteAsync_SendsDeleteToCorrectUrl()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.NoContent);
        var service = new PetApiService(CreateClient(handler));

        await service.DeleteAsync(4);

        Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
        Assert.Equal("/api/pets/4", handler.LastRequest.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task CreateAsync_OnServerError_Throws()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.BadRequest);
        var service = new PetApiService(CreateClient(handler));

        await Assert.ThrowsAsync<HttpRequestException>(() => service.CreateAsync(new PetWriteDto
        {
            Name = "X", Species = "Perro", DateOfBirth = new DateTime(2020, 1, 1), PetOwnerId = 1
        }));
    }
}
