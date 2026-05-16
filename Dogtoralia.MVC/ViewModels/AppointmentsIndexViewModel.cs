using Dogtoralia.MVC.Helpers;
using Dogtoralia.Shared.Dtos;
using Dogtoralia.Shared.Models;

namespace Dogtoralia.MVC.ViewModels;

public class AppointmentsIndexViewModel
{
    public PaginatedList<AppointmentDto> Appointments { get; set; } = null!;
    public int? SelectedClinicId { get; set; }
    public AppointmentStatus? SelectedStatus { get; set; }
    public List<ClinicDto> Clinics { get; set; } = new();
    public List<AppointmentStatus> AvailableStatuses { get; set; } = new();
}
