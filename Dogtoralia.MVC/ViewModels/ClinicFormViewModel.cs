using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dogtoralia.MVC.ViewModels;

public class ClinicFormViewModel
{
    public int? Id { get; set; }

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

    [Required]
    public int SpecialityId { get; set; }

    public SelectList? SpecialityOptions { get; set; }
}
