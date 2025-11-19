using Repository.Models;

namespace Repository.Repository
{
    public interface IRubricRepository
    {
        Task<IEnumerable<Rubric>> GetAllRubricsAsync();
        Task<IEnumerable<Rubric>> GetRubricsByExamIdAsync(long examId);
        Task<Rubric?> GetRubricByIdAsync(long id);
        Task<Rubric> CreateRubricAsync(Rubric rubric);
        Task<bool> IsExamExistsAsync(long examId);
    }
}
