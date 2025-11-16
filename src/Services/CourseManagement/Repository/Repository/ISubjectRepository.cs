using Repository.Models;

namespace Repository.Repository
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetAllSubjectsAsync();
        Task<Subject?> GetSubjectByIdAsync(long id);
        Task<Subject> CreateSubjectAsync(Subject subject);
        Task<bool> IsSubjectCodeExistsAsync(string code);
    }
}
