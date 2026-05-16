using Dogtoralia.Shared.Dtos;
using System.Net.Http.Json;

namespace Dogtoralia.MVC.Services;

public class PetService : IPetService
{
    private readonly HttpClient _http;

    public PetService(HttpClient http) => _http = http;

    public async Task<IEnumerable<PetDto>> GetAllAsync()
        => await _http.GetFromJsonAsync<IEnumerable<PetDto>>("api/pets") ?? [];

    public async Task<PetDto?> GetByIdAsync(int id)
        => await _http.GetFromJsonAsync<PetDto>($"api/pets/{id}");

    public async Task<PetDto?> CreateAsync(PetWriteDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/pets", dto);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<PetDto>()
            : null;
    }

    public async Task<bool> UpdateAsync(int id, PetWriteDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/pets/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/pets/{id}");
        return response.IsSuccessStatusCode;
    }
}
