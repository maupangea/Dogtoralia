using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Core.Services;

public interface IClinicApiService
{
    Task<IReadOnlyList<ClinicDto>> GetAllAsync();
    Task<ClinicDto?> GetByIdAsync(int id);
}
