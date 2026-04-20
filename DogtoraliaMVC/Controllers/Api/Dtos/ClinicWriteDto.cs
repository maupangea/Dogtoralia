using System.ComponentModel.DataAnnotations;

namespace DogtoraliaMVC.Controllers.Api.Dtos
{
    /// <summary>
    /// Payload for creating or updating a clinic.
    /// </summary>
    public class ClinicWriteDto
    {
        /// <summary>Gets or sets the clinic name.</summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the clinic address.</summary>
        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        /// <summary>Gets or sets the clinic phone number.</summary>
        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        /// <summary>Gets or sets the clinic email address.</summary>
        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the clinic website URL.</summary>
        [MaxLength(200)]
        public string? Website { get; set; }

        /// <summary>Gets or sets the clinic description.</summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>Gets or sets the speciality identifier for the clinic.</summary>
        [Required]
        public int SpecialityId { get; set; }
    }
}
