using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Services;
using Shared.Extension;
using Shared.Models;

namespace CourseManagement.API.Controllers
{
    /// <summary>
    /// Subjects Controller
    /// 
    /// Role Mapping (Currently Commented Out):
    /// - Admin: /api/subjects (GET, POST), /api/subjects/{id} (GET)
    /// </summary>
    [ApiController]
    [Route("api/subjects")]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly ILogger<SubjectsController> _logger;

        public SubjectsController(ISubjectService subjectService, ILogger<SubjectsController> logger)
        {
            _subjectService = subjectService;
            _logger = logger;
        }

        /// <summary>
        /// Get all subjects
        /// Role: Admin
        /// </summary>
        /// <returns>List of all subjects</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Examiner")]
        public async Task<IActionResult> GetAllSubjects()
        {
            try
            {
                var subjects = await _subjectService.GetAllSubjectsAsync();
                return this.ToApiResponse(subjects, "Subjects retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subjects");
                return this.ToErrorResponse("Error retrieving subjects", ex.Message);
            }
        }

        /// <summary>
        /// Get subject by ID - Admin only
        /// </summary>
        /// <param name="id">Subject ID</param>
        /// <returns>Subject details</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Examiner")]

        public async Task<IActionResult> GetSubjectById(long id)
        {
            try
            {
                var subject = await _subjectService.GetSubjectByIdAsync(id);
                
                if (subject == null)
                    return this.ToNotFoundResponse($"Subject with ID {id} not found");

                return this.ToApiResponse(subject, "Subject retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subject with ID {Id}", id);
                return this.ToErrorResponse("Error retrieving subject", ex.Message);
            }
        }

        /// <summary>
        /// Create a new subject - Admin only
        /// </summary>
        /// <param name="createSubjectDto">Subject data</param>
        /// <returns>Created subject</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto createSubjectDto)
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

                var subject = await _subjectService.CreateSubjectAsync(createSubjectDto);
                
                var response = new ApiResponse<SubjectDto>
                {
                    IsSuccess = true,
                    Message = "Subject created successfully",
                    Data = subject
                };
                return CreatedAtAction(nameof(GetSubjectById), new { id = subject.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Subject creation failed: {Message}", ex.Message);
                return this.ToErrorResponse("Subject creation failed", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subject");
                return this.ToErrorResponse("Error creating subject", ex.Message);
            }
        }
    }
}
