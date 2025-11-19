using Service.DTO;

namespace Service.Services
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync();
        Task<SubjectDto?> GetSubjectByIdAsync(long id);
        Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto createSubjectDto);
    }
}
