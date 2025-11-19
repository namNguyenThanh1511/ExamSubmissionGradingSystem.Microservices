using Repository.Models;

namespace Repository.Repository
{
    public interface IExamRepository
    {
        Task<IEnumerable<Exam>> GetAllExamsAsync();
        Task<IEnumerable<Exam>> GetExamsAsync(long? semesterId = null, long? subjectId = null);
        Task<Exam?> GetExamByIdAsync(long id);
        Task<Exam> CreateExamAsync(Exam exam);
        Task<bool> IsSubjectExistsAsync(long subjectId);
        Task<bool> IsSemesterExistsAsync(long semesterId);
    }
}
