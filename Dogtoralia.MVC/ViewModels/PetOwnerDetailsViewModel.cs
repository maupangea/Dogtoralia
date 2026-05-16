using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.MVC.ViewModels;

public class PetOwnerDetailsViewModel
{
    public PetOwnerDto Owner { get; set; } = null!;
    public List<PetDto> Pets { get; set; } = new();
    public string? PasswordHash { get; set; }
    public bool IsOwnProfile { get; set; }
}
