using System.ComponentModel.DataAnnotations;

namespace DogtoraliaMVC.Models;

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

    [Required]
    [MaxLength(200)]
    public string OwnerName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string OwnerEmail { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string OwnerPhone { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int Age => (int)((DateTime.Today - DateOfBirth).TotalDays / 365.25);
}
