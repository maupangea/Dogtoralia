using System.ComponentModel.DataAnnotations;
using Dogtoralia.Shared.Models;

namespace Dogtoralia.Shared.Dtos;

public class AppointmentWriteDto
{
    [Required]
    public int ClinicId { get; set; }

    [Required]
    public int PetId { get; set; }

    public int? VeterinarianId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
}
