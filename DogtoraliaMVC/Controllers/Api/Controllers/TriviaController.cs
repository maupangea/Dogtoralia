using DogtoraliaMVC.Controllers.Api.Dtos;
using DogtoraliaMVC.Controllers.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DogtoraliaMVC.Controllers.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TriviaController : ControllerBase
    {
        private readonly TriviaService service;

        public TriviaController(TriviaService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Gets a random trivia question.
        /// </summary>
        /// <returns>A random trivia question.</returns>
        [HttpGet("Random")]
        public async Task<IActionResult> Random()
        {
            TriviaQuestionDto? question = await service.GetRandomQuestion();
            return Ok(question);
        }
    }
}
