using Microsoft.AspNetCore.Mvc;
using Submission.API.DTOs;
using Submission.Repositories;
using Submission.Repositories.Entities;
using Submission.Services.CourseManagementClient;
using Submission.Services.StudentSubmissionService;
using Submission.Services.UploadService;
using System.IO.Compression;
using System.Text.Json;

namespace Submission.API.Controllers
{
    [Route("api/submissions")]
    [ApiController]
    public class SubmissionController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly AppDbContext _db;
        private readonly IStudentSubmissionService _service;
        private readonly ICourseManagementClient _courseManagementClient;
        private readonly ILogger<SubmissionController> _logger;

        public SubmissionController(
            IStorageService storageService,
            AppDbContext context,
            IStudentSubmissionService studentSubmissionService,
            ICourseManagementClient courseManagementClient,
            ILogger<SubmissionController> logger)
        {
            _storageService = storageService;
            _db = context;
            _service = studentSubmissionService;
            _courseManagementClient = courseManagementClient;
            _logger = logger;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadSubmission([FromForm] SubmissionUploadDto dto)
        {
            // --- Validate file ---
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("File is required");

            if (string.IsNullOrWhiteSpace(dto.Metadata))
                return BadRequest("Metadata is required");

            // --- Parse metadata ---
            SubmissionMetadata? metadata;
            try
            {
                metadata = JsonSerializer.Deserialize<SubmissionMetadata>(dto.Metadata);
                if (metadata == null)
                    return BadRequest("Invalid metadata JSON");
            }
            catch (Exception ex)
            {
                return BadRequest($"Metadata JSON parse error: {ex.Message}");
            }

            // --- Validate metadata fields ---
            if (string.IsNullOrWhiteSpace(metadata.StudentId))
                return BadRequest("StudentId is required");

            // tạo folder tạm
            string tempFolder = Path.Combine(Path.GetTempPath(), "submissions", Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);

            string uploadedZipPath = Path.Combine(tempFolder, dto.File.FileName);

            try
            {
                // 🔹 Lưu file upload ban đầu
                using (var fs = new FileStream(uploadedZipPath, FileMode.Create))
                    await dto.File.CopyToAsync(fs);

                // 🔹 Giải nén
                string extractDir = Path.Combine(tempFolder, "extracted");
                ZipFile.ExtractToDirectory(uploadedZipPath, extractDir);

                // 🔹 Check solution.zip
                string solutionPath = Path.Combine(extractDir, "solution.zip");
                if (!System.IO.File.Exists(solutionPath))
                    return BadRequest("solution.zip missing inside uploaded file");

                // 🔹 Check metadata.json
                string metadataJsonPath = Path.Combine(extractDir, "metadata.json");
                if (!System.IO.File.Exists(metadataJsonPath))
                    return BadRequest("metadata.json missing");

                // 🔹 Check violation_metadata.json
                string violationJsonPath = Path.Combine(extractDir, "violation_metadata.json");
                if (!System.IO.File.Exists(violationJsonPath))
                    return BadRequest("violation_metadata.json missing");

                // 🔹 Upload solution.zip lên S3
                string uploadedFileUrl;
                using (var solutionStream = System.IO.File.OpenRead(solutionPath))
                {
                    uploadedFileUrl = await _storageService.UploadFileAsync(
                        solutionStream,
                        $"{metadata.StudentId}_solution.zip",
                        "application/zip"
                    );
                }

                // 🔹 Insert DB record
                var submissionRecord = new StudentSubmission
                {
                    Id = Guid.NewGuid(),
                    StudentId = metadata.StudentId,
                    SolutionUrl = uploadedFileUrl,
                    Status = metadata.Status,
                    Note = metadata.Note,
                    IsValid = metadata.IsValid,
                    UploadAt = metadata.UploadAt
                };

                _db.Submissions.Add(submissionRecord);
                await _db.SaveChangesAsync();

                // 🔹 Tự động đồng bộ với CourseManagement nếu có ExamId
                bool syncSuccess = true;
                if (metadata.ExamId.HasValue)
                {
                    try
                    {
                        syncSuccess = await _courseManagementClient.LinkSubmissionToExamAsync(
                            submissionRecord.Id,
                            metadata.ExamId.Value,
                            metadata.StudentId
                        );
                    }
                    catch (Exception ex)
                    {
                        // Log error nhưng không fail request
                        _logger.LogError(ex,
                            "Failed to sync submission {SubmissionId} to CourseManagement",
                            submissionRecord.Id);
                        syncSuccess = false;
                    }
                }

                return Ok(new
                {
                    message = "Uploaded successfully",
                    url = uploadedFileUrl,
                    submissionId = submissionRecord.Id,
                    syncedToCourseManagement = metadata.ExamId.HasValue ? syncSuccess : (bool?)null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Processing error: {ex.Message}");
            }
            finally
            {
                // cleanup temp folder
                try { Directory.Delete(tempFolder, true); }
                catch { }
            }
        }

        [HttpPost("zip")]
        public async Task<IActionResult> UploadZip(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ");

            if (!file.FileName.EndsWith(".zip"))
                return BadRequest("Chỉ chấp nhận file .zip");

            using var stream = file.OpenReadStream();

            string fileUrl = await _storageService.UploadFileAsync(
                stream,
                file.FileName,
                file.ContentType
            );

            return Ok(new { message = "Upload thành công!", url = fileUrl });
        }

        [HttpGet("")]
        public async Task<IActionResult> GetSubmissions([FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            var result = await _service.GetSubmissionsAsync(page, size);
            return Ok(result);
        }

        [HttpGet("{submissionId}")]
        public async Task<IActionResult> GetSubmissionById([FromRoute] Guid submissionId)
        {
            try
            {
                var result = await _service.GetSubmissionByIdAsync(submissionId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Không tìm thấy submission với ID {submissionId}");
            }
        }

        // Class metadata ví dụ
        public class SubmissionMetadata
        {
            public string StudentId { get; set; } = string.Empty;
            public long? ExamId { get; set; }
            public string Status { get; set; } = string.Empty;
            public string Note { get; set; } = string.Empty;
            public bool IsValid { get; set; }
            public DateTime UploadAt { get; set; }
        }

    }
}
