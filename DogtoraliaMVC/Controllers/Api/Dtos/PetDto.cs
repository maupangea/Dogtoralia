namespace DogtoraliaMVC.Controllers.Api.Dtos
{
    /// <summary>
    /// Represents a pet returned by the API.
    /// </summary>
    public class PetDto
    {
        /// <summary>Gets or sets the pet identifier.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the pet name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the species.</summary>
        public string Species { get; set; } = string.Empty;

        /// <summary>Gets or sets the breed.</summary>
        public string Breed { get; set; } = string.Empty;

        /// <summary>Gets or sets the date of birth.</summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>Gets or sets the age in years (computed from date of birth).</summary>
        public int Age { get; set; }

        /// <summary>Gets or sets optional notes.</summary>
        public string? Notes { get; set; }

        /// <summary>Gets or sets the date the pet was created.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Gets or sets the owner identifier.</summary>
        public int PetOwnerId { get; set; }

        /// <summary>Gets or sets the owner name.</summary>
        public string PetOwnerName { get; set; } = string.Empty;
    }
}
