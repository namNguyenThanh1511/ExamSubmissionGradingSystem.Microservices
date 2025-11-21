using Repository.Models;

namespace Repository.Repository
{
    public interface ISubmissionRepository
    {
        Task<IEnumerable<Submission>> GetSubmissionsByExamIdAsync(long examId);
        Task<Submission?> GetSubmissionByIdAsync(long id);
        Task<Submission?> GetSubmissionDetailByIdAsync(long id);
        Task<Submission> CreateSubmissionAsync(Submission submission);
        Task<Submission> UpdateSubmissionAsync(Submission submission);
    }
}
