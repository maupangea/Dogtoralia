using System.ComponentModel.DataAnnotations;
using DogtoraliaMVC.Models;

namespace DogtoraliaMVC.Controllers.Api.Dtos
{
    /// <summary>
    /// Payload for creating or updating an appointment.
    /// </summary>
    public class AppointmentWriteDto
    {
        /// <summary>Gets or sets the clinic identifier.</summary>
        [Required]
        public int ClinicId { get; set; }

        /// <summary>Gets or sets the pet identifier.</summary>
        [Required]
        public int PetId { get; set; }

        /// <summary>Gets or sets the veterinarian identifier (optional).</summary>
        public int? VeterinarianId { get; set; }

        /// <summary>Gets or sets the appointment date and time.</summary>
        [Required]
        public DateTime AppointmentDate { get; set; }

        /// <summary>Gets or sets the reason for the appointment.</summary>
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        /// <summary>Gets or sets optional notes.</summary>
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>Gets or sets the appointment status.</summary>
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    }
}
