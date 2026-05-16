using System.ComponentModel.DataAnnotations;

namespace Dogtoralia.Shared.Models;

public class Pet
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Species { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Breed { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int PetOwnerId { get; set; }

    public PetOwner PetOwner { get; set; } = null!;

    public int Age => (int)((DateTime.Today - DateOfBirth).TotalDays / 365.25);

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
