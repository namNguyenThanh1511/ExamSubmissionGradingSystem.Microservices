using Microsoft.AspNetCore.Authorization;
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
        /// Tạo submission record mới - link submission từ Submission service với Exam
        /// </summary>
        /// <param name="createSubmissionDto">Submission data</param>
        /// <returns>Submission đã được tạo</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateSubmission([FromBody] CreateSubmissionDto createSubmissionDto)
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

                var submission = await _submissionService.CreateSubmissionAsync(createSubmissionDto);
                return this.ToApiResponse(submission, $"Submission linked to exam {createSubmissionDto.ExamId} successfully");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found when creating submission");
                return this.ToNotFoundResponse(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when creating submission");
                return this.ToErrorResponse("Invalid operation", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating submission");
                return this.ToErrorResponse("Error creating submission", ex.Message);
            }
        }

        /// <summary>
        /// L?y danh sch bi n?p theo examId
        /// Role: Manager
        /// </summary>
        /// <param name="examId">Exam ID</param>
        /// <returns>Danh sch bi n?p</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Examiner")]
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
        /// Lấy thông tin chi tiết chấm điểm của submission bao gồm criteria scores
        /// Role: Admin, Manager, Examiner
        /// </summary>
        /// <param name="id">Submission ID</param>
        /// <returns>Thông tin chi tiết submission với criteria scores</returns>
        [HttpGet("{id}/detail")]
        [Authorize(Roles = "Admin,Manager,Examiner")]
        public async Task<IActionResult> GetSubmissionDetail(long id)
        {
            try
            {
                var submissionDetail = await _submissionService.GetSubmissionDetailAsync(id);
                
                if (submissionDetail == null)
                {
                    return this.ToNotFoundResponse($"Submission with ID {id} not found");
                }

                return this.ToApiResponse(submissionDetail, $"Submission detail retrieved successfully for ID {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving submission detail for ID {SubmissionId}", id);
                return this.ToErrorResponse("Error retrieving submission detail", ex.Message);
            }
        }

        /// <summary>
        /// Phn cng gim kh?o cho bi n?p
        /// Role: Manager
        /// </summary>
        /// <param name="id">Submission ID</param>
        /// <param name="examinerId">Examiner ID</param>
        /// <returns>Bi n?p ? ???c c?p nh?t</returns>
        [HttpPatch("{id}/assign/{examinerId}")]
        [Authorize(Roles = "Manager")]
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
        /// Examiner ch?m ?i?m bi n?p
        /// Role: Examiner
        /// </summary>
        /// <param name="id">Submission ID</param>
        /// <param name="gradeSubmissionDto">Grade data</param>
        /// <returns>Bi n?p ? ???c ch?m ?i?m</returns>
        [HttpPatch("{id}/grade")]
        [Authorize(Roles = "Examiner")]
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

        /// <summary>
        /// Examiner chấm điểm bài nộp theo từng criteria
        /// Role: Examiner
        /// </summary>
        /// <param name="gradeByCriteriaDto">Grade by criteria data</param>
        /// <returns>Bài nộp đã được chấm điểm theo criteria</returns>
        [HttpPatch("grade-criteria")]
        [Authorize(Roles = "Examiner")]
        public async Task<IActionResult> GradeSubmissionByCriteria([FromBody] GradeSubmissionByCriteriaDto gradeByCriteriaDto)
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

                var submission = await _submissionService.GradeSubmissionByCriteriaAsync(gradeByCriteriaDto);
                return this.ToApiResponse(submission, $"Submission {gradeByCriteriaDto.SubmissionId} graded successfully by criteria");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found when grading submission by criteria");
                return this.ToNotFoundResponse(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when grading submission by criteria");
                return this.ToErrorResponse("Invalid operation", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error grading submission by criteria");
                return this.ToErrorResponse("Error grading submission by criteria", ex.Message);
            }
        }
    }
}
