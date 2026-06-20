using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Core.Services;

public interface IPetApiService
{
    Task<IReadOnlyList<PetDto>> GetAllAsync();
    Task<PetDto?> GetByIdAsync(int id);
    Task<PetDto?> CreateAsync(PetWriteDto pet);
    Task UpdateAsync(int id, PetWriteDto pet);
    Task DeleteAsync(int id);
}
