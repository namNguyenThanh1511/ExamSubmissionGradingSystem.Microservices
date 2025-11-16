using Service.DTO;

namespace Service.Services
{
    public interface ISemesterService
    {
        Task<IEnumerable<SemesterDto>> GetAllSemestersAsync();
        Task<SemesterDto?> GetSemesterByIdAsync(long id);
        Task<SemesterDto> CreateSemesterAsync(CreateSemesterDto createSemesterDto);
    }
}
