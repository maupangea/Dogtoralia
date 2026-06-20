using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Core.Services;

public interface IPetOwnerApiService
{
    Task<IReadOnlyList<PetOwnerDto>> GetAllAsync();
}
