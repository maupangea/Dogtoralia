using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.MVC.Services;

public interface IPetService
{
    Task<IEnumerable<PetDto>> GetAllAsync();
    Task<PetDto?> GetByIdAsync(int id);
    Task<PetDto?> CreateAsync(PetWriteDto dto);
    Task<bool> UpdateAsync(int id, PetWriteDto dto);
    Task<bool> DeleteAsync(int id);
}
