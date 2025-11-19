using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;

namespace Repository.Repository
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly ExamManagementContext _context;

        public SubjectRepository(ExamManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subject>> GetAllSubjectsAsync()
        {
            return await _context.Subjects
                .OrderBy(s => s.Code)
                .ToListAsync();
        }

        public async Task<Subject?> GetSubjectByIdAsync(long id)
        {
            return await _context.Subjects
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Subject> CreateSubjectAsync(Subject subject)
        {
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task<bool> IsSubjectCodeExistsAsync(string code)
        {
            return await _context.Subjects
                .AnyAsync(s => s.Code == code);
        }
    }
}
