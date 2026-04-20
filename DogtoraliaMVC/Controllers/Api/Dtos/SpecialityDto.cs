namespace DogtoraliaMVC.Controllers.Api.Dtos
{
    /// <summary>
    /// Represents a speciality returned by the API.
    /// </summary>
    public class SpecialityDto
    {
        /// <summary>Gets or sets the speciality identifier.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the speciality name.</summary>
        public string Name { get; set; } = string.Empty;
    }
}
