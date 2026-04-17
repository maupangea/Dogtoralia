using System.Text.Json.Serialization;

namespace DogtoraliaMVC.Controllers.Api.Dtos
{
    /// <summary>
    /// Represents a state data transfer object.
    /// </summary>
    public class StateDto
    {
        /// <summary>
        /// Gets or sets the state identifier.
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the state name.
        /// </summary>
        [JsonPropertyName ("nombre")]
        public string? Nombre { get; set; }
    }
}
