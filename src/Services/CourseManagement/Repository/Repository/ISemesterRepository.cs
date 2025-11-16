using Repository.Models;

namespace Repository.Repository
{
    public interface ISemesterRepository
    {
        Task<IEnumerable<Semester>> GetAllSemestersAsync();
        Task<Semester?> GetSemesterByIdAsync(long id);
        Task<Semester> CreateSemesterAsync(Semester semester);
        Task<bool> IsSemesterNameExistsAsync(string name);
    }
}
