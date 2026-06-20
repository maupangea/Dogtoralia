using System.Net.Http.Json;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Core.Services;

public class PetApiService : IPetApiService
{
    private readonly HttpClient _http;

    public PetApiService(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<PetDto>> GetAllAsync()
    {
        var pets = await _http.GetFromJsonAsync<List<PetDto>>("api/pets", ApiJson.Options);
        return pets ?? new List<PetDto>();
    }

    public Task<PetDto?> GetByIdAsync(int id) =>
        _http.GetFromJsonAsync<PetDto>($"api/pets/{id}", ApiJson.Options);

    public async Task<PetDto?> CreateAsync(PetWriteDto pet)
    {
        var response = await _http.PostAsJsonAsync("api/pets", pet, ApiJson.Options);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PetDto>(ApiJson.Options);
    }

    public async Task UpdateAsync(int id, PetWriteDto pet)
    {
        var response = await _http.PutAsJsonAsync($"api/pets/{id}", pet, ApiJson.Options);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/pets/{id}");
        response.EnsureSuccessStatusCode();
    }
}
