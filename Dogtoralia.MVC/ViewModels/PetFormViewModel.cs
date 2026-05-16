using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dogtoralia.MVC.ViewModels;

public class PetFormViewModel
{
    public int? Id { get; set; }

    [Required]
    public int PetOwnerId { get; set; }

    // Display-only: populated from the PetOwner entity, not saved directly
    public string OwnerName { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
    public string OwnerPhone { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Species { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Breed { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public SelectList SpeciesOptions { get; set; } = new SelectList(
        new[] { "Perro", "Gato", "Ave", "Conejo", "Hámster", "Otro" });
}
