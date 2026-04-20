using System.ComponentModel.DataAnnotations;

namespace DogtoraliaMVC.Controllers.Api.Dtos
{
    /// <summary>
    /// Payload for creating or updating a pet.
    /// </summary>
    public class PetWriteDto
    {
        /// <summary>Gets or sets the pet name.</summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the species.</summary>
        [Required]
        [MaxLength(50)]
        public string Species { get; set; } = string.Empty;

        /// <summary>Gets or sets the breed.</summary>
        [MaxLength(100)]
        public string Breed { get; set; } = string.Empty;

        /// <summary>Gets or sets the date of birth.</summary>
        [Required]
        public DateTime DateOfBirth { get; set; }

        /// <summary>Gets or sets optional notes.</summary>
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>Gets or sets the owner identifier.</summary>
        [Required]
        public int PetOwnerId { get; set; }
    }
}
