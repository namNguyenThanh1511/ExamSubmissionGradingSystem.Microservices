using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;

namespace Repository.Repository
{
    public class SemesterRepository : ISemesterRepository
    {
        private readonly ExamManagementContext _context;

        public SemesterRepository(ExamManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Semester>> GetAllSemestersAsync()
        {
            return await _context.Semesters
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();
        }

        public async Task<Semester?> GetSemesterByIdAsync(long id)
        {
            return await _context.Semesters
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Semester> CreateSemesterAsync(Semester semester)
        {
            _context.Semesters.Add(semester);
            await _context.SaveChangesAsync();
            return semester;
        }

        public async Task<bool> IsSemesterNameExistsAsync(string name)
        {
            return await _context.Semesters
                .AnyAsync(s => s.Name == name);
        }
    }
}
