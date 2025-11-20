using Submission.Repositories.Entities;

namespace Submission.Repositories.Repositories
{
    public interface IStudentSubmissionRepository
    {
        Task<(IEnumerable<StudentSubmission> Items, int TotalItems)> GetSubmissionsAsync(int page, int size);
        Task<StudentSubmission?> GetSubmissionByIdAsync(Guid submissionId);
    }
}
