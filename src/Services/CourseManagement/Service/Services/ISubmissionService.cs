using Service.DTO;

namespace Service.Services
{
    public interface ISubmissionService
    {
        Task<IEnumerable<SubmissionDto>> GetSubmissionsByExamIdAsync(long examId);
        Task<SubmissionDto> CreateSubmissionAsync(CreateSubmissionDto createSubmissionDto);
        Task<SubmissionDto?> AssignExaminerAsync(long submissionId, long examinerId);
        Task<SubmissionDto> GradeSubmissionAsync(long submissionId, double totalScore);
        Task<SubmissionDto> GradeSubmissionByCriteriaAsync(GradeSubmissionByCriteriaDto dto);
        Task<SubmissionDetailDto?> GetSubmissionDetailAsync(long submissionId);
    }
}
