
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValiaApiWeb.DTOs.Responses;
using ValiaApiWeb.Services.Interfaces;

namespace ValiaApiWeb.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class WorkConditionQuestionController : ControllerBase
    {
        private readonly IWorkConditionQuestionService _questionService;

        public WorkConditionQuestionController(IWorkConditionQuestionService questionService)
        {
            _questionService = questionService;
        }

        // GET: api/WorkConditionQuestion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkConditionQuestionResponse>>> GetActiveQuestions()
        {
            IEnumerable<WorkConditionQuestionResponse> questions = await _questionService.GetActiveQuestionsAsync();
            return Ok(questions);
        }
    }
}

