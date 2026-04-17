using DogtoraliaMVC.Controllers.Api.Dtos;
using System.Text.Json;

namespace DogtoraliaMVC.Controllers.Api.Services
{
    /// <summary>
    /// Service for managing zip codes and states.
    /// </summary>
    public class ZipCodeService : IZipCodeService
    {
        /// <summary>
        /// Gets all available states.
        /// </summary>
        /// <returns>A list of all states.</returns>
        public async Task<List<StateDto>> GetStates()
        {
            List<StateDto> states;
            var client = new HttpClient();
            var url = "https://utilidades.vmartinez84.xyz/api/CodigosPostales/Estados";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("accept", "application/json");
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
                states = JsonSerializer.Deserialize<List<StateDto>>(content) ?? new List<StateDto>();

                return states;
            }
            else
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        /// <summary>
        /// Gets a random zip code.
        /// </summary>
        /// <returns>A random zip code.</returns>
        public async Task<ZipCodeDto?> GetRandomZipCode()
        {
            ZipCodeDto? zipCodeDto;
            var client = new HttpClient();
            var url = "https://utilidades.vmartinez84.xyz/api/CodigosPostales/Aleatorio";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("accept", "application/json");
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
                zipCodeDto = JsonSerializer.Deserialize<ZipCodeDto>(content);

                return zipCodeDto;
            }
            else
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
