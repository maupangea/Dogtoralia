using DogtoraliaMVC.Controllers.Api.Dtos;
using System.Text.Json;

namespace DogtoraliaMVC.Controllers.Api.Services
{
    public class TriviaService : ITriviaService
    {
        public async Task<TriviaQuestionDto?> GetRandomQuestion()
        {
            var client = new HttpClient();
            var url = "https://opentdb.com/api.php?amount=1";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("accept", "application/json");
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
                var triviaResponse = JsonSerializer.Deserialize<TriviaResponseDto>(content);
                return triviaResponse?.Results?.FirstOrDefault();
            }
            else
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
