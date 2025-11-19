using Service.DTO;

namespace Service.Services
{
    public interface IExaminerService
    {
        Task<IEnumerable<ExaminerDto>> GetAllExaminersAsync();
        Task<ExaminerDto> CreateExaminerAsync(CreateExaminerDto createExaminerDto);
    }
}
