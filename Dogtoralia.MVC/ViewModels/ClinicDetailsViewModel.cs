using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.MVC.ViewModels;

public class ClinicDetailsViewModel
{
    public ClinicDto Clinic { get; set; } = null!;
    public List<VeterinarianDto> Veterinarians { get; set; } = new();
    public int AppointmentCount { get; set; }
}
