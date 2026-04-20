using DogtoraliaMVC.Models;

namespace DogtoraliaMVC.Controllers.Api.Dtos
{
    /// <summary>
    /// Represents an appointment returned by the API.
    /// </summary>
    public class AppointmentDto
    {
        /// <summary>Gets or sets the appointment identifier.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the clinic identifier.</summary>
        public int ClinicId { get; set; }

        /// <summary>Gets or sets the clinic name.</summary>
        public string ClinicName { get; set; } = string.Empty;

        /// <summary>Gets or sets the pet identifier.</summary>
        public int PetId { get; set; }

        /// <summary>Gets or sets the pet name.</summary>
        public string PetName { get; set; } = string.Empty;

        /// <summary>Gets or sets the veterinarian identifier, if assigned.</summary>
        public int? VeterinarianId { get; set; }

        /// <summary>Gets or sets the veterinarian full name, if assigned.</summary>
        public string? VeterinarianFullName { get; set; }

        /// <summary>Gets or sets the appointment date and time.</summary>
        public DateTime AppointmentDate { get; set; }

        /// <summary>Gets or sets the reason for the appointment.</summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>Gets or sets optional notes.</summary>
        public string? Notes { get; set; }

        /// <summary>Gets or sets the appointment status.</summary>
        public AppointmentStatus Status { get; set; }

        /// <summary>Gets or sets the date the appointment was created.</summary>
        public DateTime CreatedAt { get; set; }
    }
}
