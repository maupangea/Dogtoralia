using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.MVC.Services;

public interface ISpecialityService
{
    Task<IEnumerable<SpecialityDto>> GetAllAsync();
    Task<SpecialityDto?> GetByIdAsync(int id);
    Task<SpecialityDto?> CreateAsync(SpecialityWriteDto dto);
    Task<bool> UpdateAsync(int id, SpecialityWriteDto dto);
    Task<bool> DeleteAsync(int id);
}
