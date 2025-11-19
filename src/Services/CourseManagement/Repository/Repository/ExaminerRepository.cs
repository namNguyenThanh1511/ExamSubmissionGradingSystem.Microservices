using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;

namespace Repository.Repository
{
    public class ExaminerRepository : IExaminerRepository
    {
        private readonly ExamManagementContext _context;

        public ExaminerRepository(ExamManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Examiner>> GetAllExaminersAsync()
        {
            return await _context.Examiners
                .OrderBy(e => e.FullName)
                .ToListAsync();
        }

        public async Task<Examiner?> GetExaminerByIdAsync(long id)
        {
            return await _context.Examiners
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Examiner> CreateExaminerAsync(Examiner examiner)
        {
            _context.Examiners.Add(examiner);
            await _context.SaveChangesAsync();
            return examiner;
        }
    }
}
