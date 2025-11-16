using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;

namespace Repository.Repository
{
    public class RubricRepository : IRubricRepository
    {
        private readonly ExamManagementContext _context;

        public RubricRepository(ExamManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rubric>> GetAllRubricsAsync()
        {
            return await _context.Rubrics
                .Include(r => r.Exam)
                .Include(r => r.RubricCriteria)
                .OrderByDescending(r => r.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Rubric>> GetRubricsByExamIdAsync(long examId)
        {
            return await _context.Rubrics
                .Include(r => r.Exam)
                .Include(r => r.RubricCriteria)
                .Where(r => r.ExamId == examId)
                .OrderByDescending(r => r.Id)
                .ToListAsync();
        }

        public async Task<Rubric?> GetRubricByIdAsync(long id)
        {
            return await _context.Rubrics
                .Include(r => r.Exam)
                .Include(r => r.RubricCriteria)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Rubric> CreateRubricAsync(Rubric rubric)
        {
            _context.Rubrics.Add(rubric);
            await _context.SaveChangesAsync();
            
            // Reload with navigation properties
            return await GetRubricByIdAsync(rubric.Id) ?? rubric;
        }

        public async Task<bool> IsExamExistsAsync(long examId)
        {
            return await _context.Exams
                .AnyAsync(e => e.Id == examId);
        }
    }
}
