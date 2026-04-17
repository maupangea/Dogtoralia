using System.Text.Json.Serialization;

namespace DogtoraliaMVC.Controllers.Api.Dtos
{
    /// <summary>
    /// Represents a ZIP code data transfer object with postal and location information.
    /// </summary>
    public class ZipCodeDto
    {
        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        [JsonPropertyName("codigoPostal")]
        public string? CodigoPostal { get; set; }

        /// <summary>
        /// Gets or sets the municipality identifier.
        /// </summary>
        [JsonPropertyName("alcaldiaId")]
        public int AlcaldiaId { get; set; }

        /// <summary>
        /// Gets or sets the municipality name.
        /// </summary>
        [JsonPropertyName("estado")]
        public string? Estado { get; set; }

        /// <summary>
        /// Gets or sets the state identifier.
        /// </summary>
        [JsonPropertyName("estadoId")]
        public int EstadoId { get; set; }

        /// <summary>
        /// Gets or sets the municipality name.
        /// </summary>
        [JsonPropertyName("alcaldia")]
        public string? Alcaldia { get; set; }

        /// <summary>
        /// Gets or sets the type of settlement.
        /// </summary>
        [JsonPropertyName("tipoDeAsentamiento")]
        public string? TipoDeAsentamiento { get; set; }

        /// <summary>
        /// Gets or sets the settlement name.
        /// </summary>
        [JsonPropertyName("asentamiento")]
        public string? Asentamiento { get; set; }
    }
}
