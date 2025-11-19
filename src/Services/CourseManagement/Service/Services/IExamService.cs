using Service.DTO;

namespace Service.Services
{
    public interface IExamService
    {
        Task<IEnumerable<ExamDto>> GetAllExamsAsync();
        Task<IEnumerable<ExamDto>> GetExamsAsync(long? semesterId = null, long? subjectId = null);
        Task<ExamDto?> GetExamByIdAsync(long id);
        Task<ExamDto> CreateExamAsync(CreateExamDto createExamDto);
    }
}
