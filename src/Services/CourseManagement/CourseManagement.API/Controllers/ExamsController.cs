using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Services;
using Shared.Extension;
using Shared.Models;

namespace CourseManagement.API.Controllers
{
    /// <summary>
    /// Exams Controller
    /// 
    /// Role Mapping (Currently Commented Out):
    /// - Admin: /api/exams (GET, POST), /api/exams/{id} (GET)
    /// </summary>
    [ApiController]
    [Route("api/exams")]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;
        private readonly ILogger<ExamsController> _logger;

        public ExamsController(IExamService examService, ILogger<ExamsController> logger)
        {
            _examService = examService;
            _logger = logger;
        }

        /// <summary>
        /// Get all exams with optional filtering
        /// Role: Admin
        /// </summary>
        /// <param name="semesterId">Optional: Filter by semester ID</param>
        /// <param name="subjectId">Optional: Filter by subject ID</param>
        /// <returns>List of exams matching the filter criteria</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Examiner")]
        public async Task<IActionResult> GetAllExams([FromQuery] long? semesterId = null, [FromQuery] long? subjectId = null)
        {
            try
            {
                var exams = await _examService.GetExamsAsync(semesterId, subjectId);
                
                // Build descriptive message based on filters
                var message = "Exams retrieved successfully";
                var filters = new List<string>();
                
                if (semesterId.HasValue)
                    filters.Add($"semester ID {semesterId.Value}");
                
                if (subjectId.HasValue)
                    filters.Add($"subject ID {subjectId.Value}");
                
                if (filters.Any())
                    message += $" (filtered by {string.Join(" and ", filters)})";
                
                return this.ToApiResponse(exams, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exams. SemesterId: {SemesterId}, SubjectId: {SubjectId}", 
                    semesterId, subjectId);
                return this.ToErrorResponse("Error retrieving exams", ex.Message);
            }
        }

        /// <summary>
        /// Get exam by ID - Admin only
        /// </summary>
        /// <param name="id">Exam ID</param>
        /// <returns>Exam details</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Examiner")]

        public async Task<IActionResult> GetExamById(long id)
        {
            try
            {
                var exam = await _examService.GetExamByIdAsync(id);
                
                if (exam == null)
                    return this.ToNotFoundResponse($"Exam with ID {id} not found");

                return this.ToApiResponse(exam, "Exam retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam with ID {Id}", id);
                return this.ToErrorResponse("Error retrieving exam", ex.Message);
            }
        }

        /// <summary>
        /// Create a new exam - Admin only
        /// </summary>
        /// <param name="createExamDto">Exam data</param>
        /// <returns>Created exam</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateExam([FromBody] CreateExamDto createExamDto)
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

                var exam = await _examService.CreateExamAsync(createExamDto);
                
                var response = new ApiResponse<ExamDto>
                {
                    IsSuccess = true,
                    Message = "Exam created successfully",
                    Data = exam
                };
                return CreatedAtAction(nameof(GetExamById), new { id = exam.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Exam creation failed: {Message}", ex.Message);
                return this.ToErrorResponse("Exam creation failed", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exam");
                return this.ToErrorResponse("Error creating exam", ex.Message);
            }
        }
    }
}
