using System.Net;
using Dogtoralia.Maui.Core.Services;

namespace Dogtoralia.Tests.Maui.Services;

public class ClinicApiServiceTests
{
    private static HttpClient CreateClient(MockHttpMessageHandler handler) =>
        new(handler) { BaseAddress = new Uri("http://localhost:5186") };

    [Fact]
    public async Task GetAllAsync_ReturnsDeserializedClinics()
    {
        const string json = """
        [
          { "id": 1, "name": "Vet Center", "specialityName": "General", "veterinarianCount": 3 }
        ]
        """;
        var handler = new MockHttpMessageHandler(HttpStatusCode.OK, json);
        var service = new ClinicApiService(CreateClient(handler));

        var clinics = await service.GetAllAsync();

        Assert.Single(clinics);
        Assert.Equal("Vet Center", clinics[0].Name);
        Assert.Equal(3, clinics[0].VeterinarianCount);
        Assert.Equal("/api/clinics", handler.LastRequest!.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task GetByIdAsync_RequestsCorrectUrl()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.OK,
            """{ "id": 2, "name": "Vet Center", "specialityName": "General" }""");
        var service = new ClinicApiService(CreateClient(handler));

        var clinic = await service.GetByIdAsync(2);

        Assert.NotNull(clinic);
        Assert.Equal(2, clinic!.Id);
        Assert.Equal("/api/clinics/2", handler.LastRequest!.RequestUri!.AbsolutePath);
    }
}
