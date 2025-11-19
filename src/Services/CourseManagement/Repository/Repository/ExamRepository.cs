using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;

namespace Repository.Repository
{
    public class ExamRepository : IExamRepository
    {
        private readonly ExamManagementContext _context;

        public ExamRepository(ExamManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Exam>> GetAllExamsAsync()
        {
            return await _context.Exams
                .Include(e => e.Subject)
                .Include(e => e.Semester)
                .OrderByDescending(e => e.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetExamsAsync(long? semesterId = null, long? subjectId = null)
        {
            var query = _context.Exams
                .Include(e => e.Subject)
                .Include(e => e.Semester)
                .AsQueryable();

            // Apply semester filter if provided
            if (semesterId.HasValue)
            {
                query = query.Where(e => e.SemesterId == semesterId.Value);
            }

            // Apply subject filter if provided
            if (subjectId.HasValue)
            {
                query = query.Where(e => e.SubjectId == subjectId.Value);
            }

            return await query
                .OrderByDescending(e => e.StartTime)
                .ToListAsync();
        }

        public async Task<Exam?> GetExamByIdAsync(long id)
        {
            return await _context.Exams
                .Include(e => e.Subject)
                .Include(e => e.Semester)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Exam> CreateExamAsync(Exam exam)
        {
            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();
            
            // Reload with navigation properties
            return await GetExamByIdAsync(exam.Id) ?? exam;
        }

        public async Task<bool> IsSubjectExistsAsync(long subjectId)
        {
            return await _context.Subjects
                .AnyAsync(s => s.Id == subjectId);
        }

        public async Task<bool> IsSemesterExistsAsync(long semesterId)
        {
            return await _context.Semesters
                .AnyAsync(s => s.Id == semesterId);
        }
    }
}
