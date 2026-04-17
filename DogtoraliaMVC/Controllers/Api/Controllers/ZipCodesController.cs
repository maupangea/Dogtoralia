using DogtoraliaMVC.Controllers.Api.Dtos;
using DogtoraliaMVC.Controllers.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DogtoraliaMVC.Controllers.Api.Controllers
{
    /// <summary>
    /// API controller for managing zip codes and states.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ZipCodesController : ControllerBase
    {
        private readonly ZipCodeService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipCodesController"/> class.
        /// </summary>
        /// <param name="service">The zip code service.</param>
        public ZipCodesController(ZipCodeService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Gets all available states.
        /// </summary>
        /// <returns>A list of all states.</returns>
        [HttpGet("States")]
        public async Task<IActionResult> Get() {
            List<StateDto> states;

            states = await service.GetStates();

            return Ok(states);
        }

        /// <summary>
        /// Gets a random zip code.
        /// </summary>
        /// <returns>A random zip code.</returns>
        [HttpGet("Random")]
        public async Task<IActionResult> Random() { 
            ZipCodeDto? zipCodeDto = await service.GetRandomZipCode();

            return Ok(zipCodeDto);
        }
    }
}
