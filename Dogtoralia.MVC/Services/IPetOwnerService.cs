using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.MVC.Services;

public interface IPetOwnerService
{
    Task<IEnumerable<PetOwnerDto>> GetAllAsync();
    Task<PetOwnerDto?> GetByIdAsync(int id);
    Task<PetOwnerDto?> CreateAsync(PetOwnerWriteDto dto);
    Task<bool> UpdateAsync(int id, PetOwnerWriteDto dto);
    Task<bool> DeleteAsync(int id);
}
