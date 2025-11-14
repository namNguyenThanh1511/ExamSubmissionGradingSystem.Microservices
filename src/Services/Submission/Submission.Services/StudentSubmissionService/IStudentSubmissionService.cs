using Submission.Services.DTOs;

namespace Submission.Services.StudentSubmissionService
{
    public interface IStudentSubmissionService
    {
        Task<DTOs.PaginatedResponse<StudentSubmissionDto>> GetSubmissionsAsync(int page, int size);
    }
}
