using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Services;
using Shared.Extension;

namespace CourseManagement.API.Controllers
{
    /// <summary>
    /// Rubrics Controller
    /// 
    /// Role Mapping (Currently Commented Out):
    /// - Admin: /api/rubrics/{examId} (GET), /api/rubrics (POST)
    /// </summary>
    [ApiController]
    [Route("api/rubrics")]
    public class RubricsController : ControllerBase
    {
        private readonly IRubricService _rubricService;
        private readonly ILogger<RubricsController> _logger;

        public RubricsController(IRubricService rubricService, ILogger<RubricsController> logger)
        {
            _rubricService = rubricService;
            _logger = logger;
        }

        /// <summary>
        /// Get rubrics by exam ID - Xem rubric theo k? thi
        /// Role: Admin
        /// </summary>
        /// <param name="examId">Exam ID</param>
        /// <returns>List of rubrics for the specified exam</returns>
        [HttpGet("{examId}")]
        [Authorize(Roles = "Admin,Manager,Examiner")]
        public async Task<IActionResult> GetRubricsByExamId(long examId)
        {
            try
            {
                var rubrics = await _rubricService.GetRubricsByExamIdAsync(examId);
                return this.ToApiResponse(rubrics, $"Rubrics for exam ID {examId} retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rubrics for exam ID {ExamId}", examId);
                return this.ToErrorResponse("Error retrieving rubrics", ex.Message);
            }
        }

        /// <summary>
        /// Create a new rubric with criteria - T?o rubric cho exam (bao g?m list RubricCriterionDto)
        /// </summary>
        /// <param name="createRubricDto">Rubric data including list of criteria</param>
        /// <returns>Created rubric with criteria</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]

        public async Task<IActionResult> CreateRubric([FromBody] CreateRubricDto createRubricDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray();
                    return this.ToErrorResponse("Invalid data", errors);
                }

                var rubric = await _rubricService.CreateRubricAsync(createRubricDto);

                return this.ToApiResponse(rubric, "Rubric created successfully");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Rubric creation failed: {Message}", ex.Message);
                return this.ToErrorResponse("Rubric creation failed", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating rubric");
                return this.ToErrorResponse("Error creating rubric", ex.Message);
            }
        }
    }
}
