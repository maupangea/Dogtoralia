using System.ComponentModel.DataAnnotations;

namespace DogtoraliaMVC.Models;

public class Appointment
{
    public int Id { get; set; }

    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;

    public int? VeterinarianId { get; set; }
    public Veterinarian? Veterinarian { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
