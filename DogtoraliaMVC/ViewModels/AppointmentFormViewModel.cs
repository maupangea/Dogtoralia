using System.ComponentModel.DataAnnotations;
using DogtoraliaMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DogtoraliaMVC.ViewModels;

public class AppointmentFormViewModel
{
    public int? Id { get; set; }

    [Required]
    public int ClinicId { get; set; }

    [Required]
    public int PetId { get; set; }

    public int? VeterinarianId { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Notes { get; set; }

    [Required]
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    public SelectList? ClinicOptions { get; set; }
    public SelectList? PetOptions { get; set; }
    public SelectList? VeterinarianOptions { get; set; }
    public SelectList? StatusOptions { get; set; }
}
