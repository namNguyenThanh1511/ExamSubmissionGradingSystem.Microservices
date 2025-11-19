using Service.DTO;

namespace Service.Services
{
    public interface ISubmissionService
    {
        Task<IEnumerable<SubmissionDto>> GetSubmissionsByExamIdAsync(long examId);
        Task<SubmissionDto?> AssignExaminerAsync(long submissionId, long examinerId);
        Task<SubmissionDto> GradeSubmissionAsync(long submissionId, double totalScore);
    }
}
