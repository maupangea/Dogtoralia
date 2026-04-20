using System.Text.Json.Serialization;

namespace DogtoraliaMVC.Controllers.Api.Dtos
{
    public class TriviaResponseDto
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("results")]
        public List<TriviaQuestionDto>? Results { get; set; }
    }
}
