using DogtoraliaMVC.Helpers;
using DogtoraliaMVC.Models;

namespace DogtoraliaMVC.ViewModels;

public class PetsIndexViewModel
{
    public PaginatedList<Pet> Pets { get; set; } = null!;
    public List<string> AvailableSpecies { get; set; } = new();
    public string? SelectedSpecies { get; set; }
}
