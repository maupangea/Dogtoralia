using DogtoraliaMVC.Models;

namespace DogtoraliaMVC.ViewModels;

public class PetOwnerDetailsViewModel
{
    public PetOwner Owner { get; set; } = null!;
    public List<Pet> Pets { get; set; } = new();
}
