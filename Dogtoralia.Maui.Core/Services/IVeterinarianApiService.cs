using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Core.Services;

public interface IVeterinarianApiService
{
    Task<IReadOnlyList<VeterinarianDto>> GetAllAsync();
    Task<VeterinarianDto?> GetByIdAsync(int id);
}
