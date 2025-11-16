using Repository.Models;

namespace Repository.Repository
{
    public interface IExaminerRepository
    {
        Task<IEnumerable<Examiner>> GetAllExaminersAsync();
        Task<Examiner?> GetExaminerByIdAsync(long id);
        Task<Examiner> CreateExaminerAsync(Examiner examiner);
    }
}
