using DogtoraliaMVC.Controllers.Api.Dtos;

namespace DogtoraliaMVC.Controllers.Api.Services
{
    public interface ITriviaService
    {
        Task<TriviaQuestionDto?> GetRandomQuestion();
    }
}
