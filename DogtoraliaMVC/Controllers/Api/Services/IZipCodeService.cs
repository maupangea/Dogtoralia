using DogtoraliaMVC.Controllers.Api.Dtos;

namespace DogtoraliaMVC.Controllers.Api.Services
{
    /// <summary>
    /// Interface for zip code service operations.
    /// </summary>
    public interface IZipCodeService
    {
        /// <summary>
        /// Gets all available states.
        /// </summary>
        /// <returns>A list of all states.</returns>
        Task<List<StateDto>> GetStates();

        /// <summary>
        /// Gets a random zip code.
        /// </summary>
        /// <returns>A random zip code.</returns>
        Task<ZipCodeDto?> GetRandomZipCode();
    }
}
