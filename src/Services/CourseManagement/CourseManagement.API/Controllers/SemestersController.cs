using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Services;
using Shared.Extension;
using Shared.Models;

namespace CourseManagement.API.Controllers
{
    /// <summary>
    /// Semesters Controller
    /// 
    /// Role Mapping (Currently Commented Out):
    /// - Admin: /api/semesters (GET, POST), /api/semesters/{id} (GET)
    /// </summary>
    [ApiController]
    [Route("api/semesters")]
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _semesterService;
        private readonly ILogger<SemestersController> _logger;

        public SemestersController(ISemesterService semesterService, ILogger<SemestersController> logger)
        {
            _semesterService = semesterService;
            _logger = logger;
        }

        /// <summary>
        /// Get all semesters
        /// Role: Admin
        /// </summary>
        /// <returns>List of all semesters</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Examiner")]
        public async Task<IActionResult> GetAllSemesters()
        {
            try
            {
                var semesters = await _semesterService.GetAllSemestersAsync();
                return this.ToApiResponse(semesters, "Semesters retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving semesters");
                return this.ToErrorResponse("Error retrieving semesters", ex.Message);
            }
        }

        /// <summary>
        /// Get semester by ID - Admin only
        /// </summary>
        /// <param name="id">Semester ID</param>
        /// <returns>Semester details</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Examiner")]

        public async Task<IActionResult> GetSemesterById(long id)
        {
            try
            {
                var semester = await _semesterService.GetSemesterByIdAsync(id);
                
                if (semester == null)
                    return this.ToNotFoundResponse($"Semester with ID {id} not found");

                return this.ToApiResponse(semester, "Semester retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving semester with ID {Id}", id);
                return this.ToErrorResponse("Error retrieving semester", ex.Message);
            }
        }

        /// <summary>
        /// Create a new semester - Admin only
        /// </summary>
        /// <param name="createSemesterDto">Semester data</param>
        /// <returns>Created semester</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateSemester([FromBody] CreateSemesterDto createSemesterDto)
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

                var semester = await _semesterService.CreateSemesterAsync(createSemesterDto);
                
                var response = new ApiResponse<SemesterDto>
                {
                    IsSuccess = true,
                    Message = "Semester created successfully",
                    Data = semester
                };
                return CreatedAtAction(nameof(GetSemesterById), new { id = semester.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Semester creation failed: {Message}", ex.Message);
                return this.ToErrorResponse("Semester creation failed", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating semester");
                return this.ToErrorResponse("Error creating semester", ex.Message);
            }
        }
    }
}
