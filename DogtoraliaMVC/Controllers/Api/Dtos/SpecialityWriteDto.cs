using System.ComponentModel.DataAnnotations;

namespace DogtoraliaMVC.Controllers.Api.Dtos
{
    /// <summary>
    /// Payload for creating or updating a speciality.
    /// </summary>
    public class SpecialityWriteDto
    {
        /// <summary>Gets or sets the speciality name.</summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
