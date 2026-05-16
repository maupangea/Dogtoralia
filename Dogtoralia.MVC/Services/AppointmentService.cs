using Dogtoralia.Shared.Dtos;
using Dogtoralia.Shared.Models;
using System.Net.Http.Json;

namespace Dogtoralia.MVC.Services;

public class AppointmentService : IAppointmentService
{
    private readonly HttpClient _http;

    public AppointmentService(HttpClient http) => _http = http;

    public async Task<IEnumerable<AppointmentDto>> GetAllAsync(int? clinicId = null, AppointmentStatus? status = null)
    {
        var query = new List<string>();
        if (clinicId.HasValue) query.Add($"clinicId={clinicId.Value}");
        if (status.HasValue) query.Add($"status={(int)status.Value}");
        var url = query.Count > 0 ? $"api/appointments?{string.Join("&", query)}" : "api/appointments";
        return await _http.GetFromJsonAsync<IEnumerable<AppointmentDto>>(url) ?? [];
    }

    public async Task<AppointmentDto?> GetByIdAsync(int id)
        => await _http.GetFromJsonAsync<AppointmentDto>($"api/appointments/{id}");

    public async Task<AppointmentDto?> CreateAsync(AppointmentWriteDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/appointments", dto);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<AppointmentDto>()
            : null;
    }

    public async Task<bool> UpdateAsync(int id, AppointmentWriteDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/appointments/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/appointments/{id}");
        return response.IsSuccessStatusCode;
    }
}
