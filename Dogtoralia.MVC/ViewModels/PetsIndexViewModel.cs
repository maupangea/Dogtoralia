using Dogtoralia.MVC.Helpers;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.MVC.ViewModels;

public class PetsIndexViewModel
{
    public PaginatedList<PetDto> Pets { get; set; } = null!;
    public List<string> AvailableSpecies { get; set; } = new();
    public string? SelectedSpecies { get; set; }
}
