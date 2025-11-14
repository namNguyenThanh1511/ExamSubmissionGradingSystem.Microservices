using Microsoft.EntityFrameworkCore;
using Submission.Repositories.Entities;

namespace Submission.Repositories.Repositories
{
    public class StudentSubmissionRepository : IStudentSubmissionRepository
    {
        private readonly AppDbContext _context;
        public StudentSubmissionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<StudentSubmission> CreateAsync(StudentSubmission obj)
        {
            _context.Submissions.Add(obj);
            await _context.SaveChangesAsync();
            return obj;
        }

        public async Task<IEnumerable<StudentSubmission>> GetAll()
        {
            return await _context.Submissions.ToListAsync();
        }

        public async Task<StudentSubmission?> GetByIdAsync(Guid id)
        {
            return await _context.Submissions
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(StudentSubmission obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var obj = await _context.Submissions.FindAsync(id);
            if (obj == null) return false;

            _context.Submissions.Remove(obj);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(IEnumerable<StudentSubmission> Items, int TotalItems)> GetSubmissionsAsync(int page, int size)
        {
            var query = _context.Submissions.AsQueryable();

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(s => s.UploadAt)
                .Skip(page * size)
                .Take(size)
                .ToListAsync();

            return (items, totalItems);
        }

    }
}
