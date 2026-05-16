using Dogtoralia.Shared.Dtos;
using System.Net.Http.Json;

namespace Dogtoralia.MVC.Services;

public class PetOwnerService : IPetOwnerService
{
    private readonly HttpClient _http;

    public PetOwnerService(HttpClient http) => _http = http;

    public async Task<IEnumerable<PetOwnerDto>> GetAllAsync()
        => await _http.GetFromJsonAsync<IEnumerable<PetOwnerDto>>("api/petowners") ?? [];

    public async Task<PetOwnerDto?> GetByIdAsync(int id)
        => await _http.GetFromJsonAsync<PetOwnerDto>($"api/petowners/{id}");

    public async Task<PetOwnerDto?> CreateAsync(PetOwnerWriteDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/petowners", dto);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<PetOwnerDto>()
            : null;
    }

    public async Task<bool> UpdateAsync(int id, PetOwnerWriteDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/petowners/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/petowners/{id}");
        return response.IsSuccessStatusCode;
    }
}
