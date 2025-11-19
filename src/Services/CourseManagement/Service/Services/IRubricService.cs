using Service.DTO;

namespace Service.Services
{
    public interface IRubricService
    {
        Task<IEnumerable<RubricDto>> GetAllRubricsAsync();
        Task<IEnumerable<RubricDto>> GetRubricsByExamIdAsync(long examId);
        Task<RubricDto?> GetRubricByIdAsync(long id);
        Task<RubricDto> CreateRubricAsync(CreateRubricDto createRubricDto);
    }
}
