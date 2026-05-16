using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.MVC.Services;

public interface IVeterinarianService
{
    Task<IEnumerable<VeterinarianDto>> GetAllAsync();
    Task<VeterinarianDto?> GetByIdAsync(int id);
    Task<VeterinarianDto?> CreateAsync(VeterinarianWriteDto dto);
    Task<bool> UpdateAsync(int id, VeterinarianWriteDto dto);
    Task<bool> DeleteAsync(int id);
}
