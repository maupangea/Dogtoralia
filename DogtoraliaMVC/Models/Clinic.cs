using System.ComponentModel.DataAnnotations;

namespace DogtoraliaMVC.Models;

public class Clinic
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Website { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int SpecialityId { get; set; }
    public Speciality Speciality { get; set; } = null!;

    public ICollection<Veterinarian> Veterinarians { get; set; } = new List<Veterinarian>();
}
