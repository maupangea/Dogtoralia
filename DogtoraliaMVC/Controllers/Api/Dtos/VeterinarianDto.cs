namespace DogtoraliaMVC.Controllers.Api.Dtos
{
    /// <summary>
    /// Represents a veterinarian returned by the API.
    /// </summary>
    public class VeterinarianDto
    {
        /// <summary>Gets or sets the veterinarian identifier.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the first name.</summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>Gets or sets the last name.</summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>Gets or sets the full name.</summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>Gets or sets the professional license number.</summary>
        public string LicenseNumber { get; set; } = string.Empty;

        /// <summary>Gets or sets the email address.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the phone number.</summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>Gets or sets the years of experience.</summary>
        public int YearsOfExperience { get; set; }

        /// <summary>Gets or sets the associated clinic identifier.</summary>
        public int ClinicId { get; set; }

        /// <summary>Gets or sets the associated clinic name.</summary>
        public string ClinicName { get; set; } = string.Empty;
    }
}
