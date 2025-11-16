using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Services;
using Shared.Extension;

namespace CourseManagement.API.Controllers
{
    /// <summary>
    /// Submissions Controller
    /// 
    /// Role Mapping (Currently Commented Out):
    /// - Manager: /api/submissions (GET), /api/submissions/{id}/assign/{examinerId} (PATCH)
    /// - Examiner: /api/submissions/{id}/grade (PATCH)
    /// </summary>
    [ApiController]
    [Route("api/submissions")]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;
        private readonly ILogger<SubmissionsController> _logger;

        public SubmissionsController(ISubmissionService submissionService, ILogger<SubmissionsController> logger)
        {
            _submissionService = submissionService;
            _logger = logger;
        }

        /// <summary>
        /// L?y danh sách bài n?p theo examId
        /// Role: Manager
        /// </summary>
        /// <param name="examId">Exam ID</param>
        /// <returns>Danh sách bài n?p</returns>
        [HttpGet]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetSubmissions([FromQuery] long examId)
        {
            try
            {
                var submissions = await _submissionService.GetSubmissionsByExamIdAsync(examId);
                return this.ToApiResponse(submissions, $"Submissions retrieved successfully for exam ID {examId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving submissions for exam ID {ExamId}", examId);
                return this.ToErrorResponse("Error retrieving submissions", ex.Message);
            }
        }

        /// <summary>
        /// Phân công giám kh?o cho bài n?p
        /// Role: Manager
        /// </summary>
        /// <param name="id">Submission ID</param>
        /// <param name="examinerId">Examiner ID</param>
        /// <returns>Bài n?p ?ã ???c c?p nh?t</returns>
        [HttpPatch("{id}/assign/{examinerId}")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> AssignExaminer(long id, long examinerId)
        {
            try
            {
                var submission = await _submissionService.AssignExaminerAsync(id, examinerId);
                return this.ToApiResponse(submission, $"Examiner {examinerId} assigned to submission {id} successfully");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found when assigning examiner {ExaminerId} to submission {SubmissionId}", examinerId, id);
                return this.ToNotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning examiner {ExaminerId} to submission {SubmissionId}", examinerId, id);
                return this.ToErrorResponse("Error assigning examiner", ex.Message);
            }
        }

        /// <summary>
        /// Examiner ch?m ?i?m bài n?p
        /// Role: Examiner
        /// </summary>
        /// <param name="id">Submission ID</param>
        /// <param name="gradeSubmissionDto">Grade data</param>
        /// <returns>Bài n?p ?ã ???c ch?m ?i?m</returns>
        [HttpPatch("{id}/grade")]
        //[Authorize(Roles = "Examiner")]
        public async Task<IActionResult> GradeSubmission(long id, [FromBody] GradeSubmissionDto gradeSubmissionDto)
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

                var submission = await _submissionService.GradeSubmissionAsync(id, gradeSubmissionDto.TotalScore);
                return this.ToApiResponse(submission, $"Submission {id} graded successfully with score {gradeSubmissionDto.TotalScore}");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Submission not found when grading submission ID {SubmissionId}", id);
                return this.ToNotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error grading submission ID {SubmissionId}", id);
                return this.ToErrorResponse("Error grading submission", ex.Message);
            }
        }
    }
}
