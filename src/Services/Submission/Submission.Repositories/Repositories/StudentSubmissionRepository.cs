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

        public async Task<StudentSubmission?> GetSubmissionByIdAsync(Guid submissionId)
        {
            return await _context.Submissions
                .FirstOrDefaultAsync(s => s.Id == submissionId);
        }

    }
}
