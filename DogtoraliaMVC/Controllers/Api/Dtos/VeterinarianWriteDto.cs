using System.ComponentModel.DataAnnotations;

namespace DogtoraliaMVC.Controllers.Api.Dtos
{
    /// <summary>
    /// Payload for creating or updating a veterinarian.
    /// </summary>
    public class VeterinarianWriteDto
    {
        /// <summary>Gets or sets the first name.</summary>
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>Gets or sets the last name.</summary>
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>Gets or sets the professional license number.</summary>
        [Required]
        [MaxLength(20)]
        public string LicenseNumber { get; set; } = string.Empty;

        /// <summary>Gets or sets the email address.</summary>
        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the phone number.</summary>
        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        /// <summary>Gets or sets the years of experience.</summary>
        [Range(0, 60)]
        public int YearsOfExperience { get; set; }

        /// <summary>Gets or sets the clinic identifier.</summary>
        [Required]
        public int ClinicId { get; set; }
    }
}
