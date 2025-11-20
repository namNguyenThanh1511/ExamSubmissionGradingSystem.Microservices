using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Submission.Services.CourseManagementClient
{
    public class CourseManagementClient : ICourseManagementClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CourseManagementClient> _logger;
        private readonly string _baseUrl;

        public CourseManagementClient(
            HttpClient httpClient,
            ILogger<CourseManagementClient> logger,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _baseUrl = configuration["CourseManagement:BaseUrl"] ?? "http://localhost:5066";
        }

        public async Task<bool> LinkSubmissionToExamAsync(Guid submissionId, long examId, string studentCode)
        {
            try
            {
                var request = new
                {
                    ExamId = examId,
                    SubmissionId = submissionId,
                    StudentCode = studentCode
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"{_baseUrl}/api/submissions",
                    request
                );

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(
                        "Successfully linked submission {SubmissionId} to exam {ExamId}",
                        submissionId, examId);
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning(
                        "Failed to link submission {SubmissionId} to exam {ExamId}. Status: {Status}, Error: {Error}",
                        submissionId, examId, response.StatusCode, errorContent);
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    "HTTP error linking submission {SubmissionId} to exam {ExamId}",
                    submissionId, examId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error linking submission {SubmissionId} to exam {ExamId}",
                    submissionId, examId);
                return false;
            }
        }
    }
}

