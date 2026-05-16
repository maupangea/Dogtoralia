using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.MVC.Services;

public interface IClinicService
{
    Task<IEnumerable<ClinicDto>> GetAllAsync();
    Task<ClinicDto?> GetByIdAsync(int id);
    Task<ClinicDto?> CreateAsync(ClinicWriteDto dto);
    Task<bool> UpdateAsync(int id, ClinicWriteDto dto);
    Task<bool> DeleteAsync(int id);
}
