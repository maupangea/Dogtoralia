using DogtoraliaMVC.Helpers;
using DogtoraliaMVC.Models;

namespace DogtoraliaMVC.ViewModels;

public class ClinicsIndexViewModel
{
    public PaginatedList<Clinic> Clinics { get; set; } = null!;
    public List<Speciality> Specialities { get; set; } = new();
    public int? SelectedSpecialityId { get; set; }
}
