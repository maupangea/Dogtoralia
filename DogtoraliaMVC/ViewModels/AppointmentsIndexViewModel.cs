using DogtoraliaMVC.Helpers;
using DogtoraliaMVC.Models;

namespace DogtoraliaMVC.ViewModels;

public class AppointmentsIndexViewModel
{
    public PaginatedList<Appointment> Appointments { get; set; } = null!;
    public int? SelectedClinicId { get; set; }
    public AppointmentStatus? SelectedStatus { get; set; }
    public List<Clinic> Clinics { get; set; } = new();
    public List<AppointmentStatus> AvailableStatuses { get; set; } = new();
}
