namespace DogtoraliaMVC.Controllers.Api.Dtos
{
    /// <summary>
    /// Represents a pet owner returned by the API.
    /// </summary>
    public class PetOwnerDto
    {
        /// <summary>Gets or sets the pet owner identifier.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the full name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the email address.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the phone number.</summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>Gets or sets the date the owner was created.</summary>
        public DateTime CreatedAt { get; set; }
    }
}
