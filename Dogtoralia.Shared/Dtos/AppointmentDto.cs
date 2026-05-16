using Dogtoralia.Shared.Models;

namespace Dogtoralia.Shared.Dtos;

public class AppointmentDto
{
    public int Id { get; set; }
    public int ClinicId { get; set; }
    public string ClinicName { get; set; } = string.Empty;
    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;
    public string PetSpecies { get; set; } = string.Empty;
    public string PetBreed { get; set; } = string.Empty;
    public int PetOwnerId { get; set; }
    public int? VeterinarianId { get; set; }
    public string? VeterinarianFullName { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public AppointmentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
