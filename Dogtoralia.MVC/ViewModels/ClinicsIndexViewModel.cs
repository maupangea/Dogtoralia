using Dogtoralia.MVC.Helpers;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.MVC.ViewModels;

public class ClinicsIndexViewModel
{
    public PaginatedList<ClinicDto> Clinics { get; set; } = null!;
    public List<SpecialityDto> Specialities { get; set; } = new();
    public int? SelectedSpecialityId { get; set; }
}
