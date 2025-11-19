using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;

namespace Repository.Repository
{
    public class ViolationRepository : IViolationRepository
    {
        private readonly ExamManagementContext _context;

        public ViolationRepository(ExamManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Violation>> GetViolationsByStatusAsync(bool? status)
        {
            var query = _context.Violations
                .Include(v => v.Submission)
                    .ThenInclude(s => s!.Exam)
                .Include(v => v.Submission)
                    .ThenInclude(s => s!.Examiner)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(v => v.Verified == status.Value);
            }

            return await query
                .OrderByDescending(v => v.Id)
                .ToListAsync();
        }

        public async Task<Violation?> GetViolationByIdAsync(long id)
        {
            return await _context.Violations
                .Include(v => v.Submission)
                    .ThenInclude(s => s!.Exam)
                .Include(v => v.Submission)
                    .ThenInclude(s => s!.Examiner)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Violation> CreateViolationAsync(Violation violation)
        {
            _context.Violations.Add(violation);
            await _context.SaveChangesAsync();
            return violation;
        }

        public async Task<Violation> UpdateViolationAsync(Violation violation)
        {
            _context.Violations.Update(violation);
            await _context.SaveChangesAsync();
            return violation;
        }
    }
}
