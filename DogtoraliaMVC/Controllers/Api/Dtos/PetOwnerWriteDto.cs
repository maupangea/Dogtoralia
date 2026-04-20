using System.ComponentModel.DataAnnotations;

namespace DogtoraliaMVC.Controllers.Api.Dtos
{
    /// <summary>
    /// Payload for creating or updating a pet owner.
    /// </summary>
    public class PetOwnerWriteDto
    {
        /// <summary>Gets or sets the full name.</summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the email address.</summary>
        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the phone number.</summary>
        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
    }
}
