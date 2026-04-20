namespace DogtoraliaMVC.Controllers.Api.Dtos
{
    /// <summary>
    /// Represents a clinic returned by the API.
    /// </summary>
    public class ClinicDto
    {
        /// <summary>Gets or sets the clinic identifier.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the clinic name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the clinic address.</summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>Gets or sets the clinic phone number.</summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>Gets or sets the clinic email address.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the clinic website URL.</summary>
        public string? Website { get; set; }

        /// <summary>Gets or sets the clinic description.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets the date the clinic was created.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Gets or sets the associated speciality identifier.</summary>
        public int SpecialityId { get; set; }

        /// <summary>Gets or sets the associated speciality name.</summary>
        public string SpecialityName { get; set; } = string.Empty;
    }
}
