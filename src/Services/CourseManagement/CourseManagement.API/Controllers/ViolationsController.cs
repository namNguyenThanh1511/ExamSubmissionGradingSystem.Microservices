using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Services;
using Shared.Extension;

namespace CourseManagement.API.Controllers
{
    /// <summary>
    /// Violations Controller
    /// 
    /// Role Mapping (Currently Commented Out):
    /// - Manager: /api/violations (GET, POST)
    /// - Moderator: /api/violations/{id}/verify (PATCH)
    /// </summary>
    [ApiController]
    [Route("api/violations")]
    public class ViolationsController : ControllerBase
    {
        private readonly IViolationService _violationService;
        private readonly ILogger<ViolationsController> _logger;

        public ViolationsController(IViolationService violationService, ILogger<ViolationsController> logger)
        {
            _violationService = violationService;
            _logger = logger;
        }

        /// <summary>
        /// L?y danh sách vi ph?m (có th? l?c theo tr?ng thái verified)
        /// Role: Manager
        /// </summary>
        /// <param name="status">Optional: Filter by verified status (true/false)</param>
        /// <returns>Danh sách vi ph?m</returns>
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetViolations([FromQuery] bool? status = null)
        {
            try
            {
                var violations = await _violationService.GetViolationsByStatusAsync(status);
                
                var message = status.HasValue 
                    ? $"Violations retrieved successfully (verified: {status.Value})" 
                    : "All violations retrieved successfully";
                
                return this.ToApiResponse(violations, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving violations with status filter: {Status}", status);
                return this.ToErrorResponse("Error retrieving violations", ex.Message);
            }
        }

        /// <summary>
        /// Ghi nh?n vi ph?m m?i
        /// Role: Manager
        /// </summary>
        /// <param name="createViolationDto">Violation data</param>
        /// <returns>Vi ph?m ?ã ???c t?o</returns>
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateViolation([FromBody] CreateViolationDto createViolationDto)
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

                var violation = await _violationService.CreateViolationAsync(createViolationDto);
                return this.ToApiResponse(violation, "Violation created successfully");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Submission not found when creating violation for submission ID {SubmissionId}", createViolationDto.SubmissionId);
                return this.ToNotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating violation for submission ID {SubmissionId}", createViolationDto.SubmissionId);
                return this.ToErrorResponse("Error creating violation", ex.Message);
            }
        }

        /// <summary>
        /// Moderator xác minh vi ph?m
        /// Role: Moderator
        /// </summary>
        /// <param name="id">Violation ID</param>
        /// <param name="verifyViolationDto">Verification data</param>
        /// <returns>Vi ph?m ?ã ???c c?p nh?t</returns>
        [HttpPatch("{id}/verify")]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> VerifyViolation(long id, [FromBody] VerifyViolationDto verifyViolationDto)
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

                var violation = await _violationService.VerifyViolationAsync(id, verifyViolationDto.Verified);
                
                var message = verifyViolationDto.Verified 
                    ? $"Violation {id} has been verified successfully" 
                    : $"Violation {id} verification has been revoked";
                
                return this.ToApiResponse(violation, message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Violation not found when verifying violation ID {ViolationId}", id);
                return this.ToNotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying violation ID {ViolationId}", id);
                return this.ToErrorResponse("Error verifying violation", ex.Message);
            }
        }
    }
}
