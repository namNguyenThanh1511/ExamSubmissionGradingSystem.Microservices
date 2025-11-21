using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;

namespace Repository.Repository
{
    public class SubmissionRepository : ISubmissionRepository
    {
        private readonly ExamManagementContext _context;

        public SubmissionRepository(ExamManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Submission>> GetSubmissionsByExamIdAsync(long examId)
        {
            return await _context.Submissions
                .Include(s => s.Exam)
                .Include(s => s.Examiner)
                .Where(s => s.ExamId == examId)
                .OrderBy(s => s.Id)
                .ToListAsync();
        }

        public async Task<Submission?> GetSubmissionByIdAsync(long id)
        {
            return await _context.Submissions
                .Include(s => s.Exam)
                .Include(s => s.Examiner)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Submission?> GetSubmissionDetailByIdAsync(long id)
        {
            return await _context.Submissions
                .Include(s => s.Exam)
                .Include(s => s.Examiner)
                .Include(s => s.CriterionScores)
                    .ThenInclude(cs => cs.RubricCriterion)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Submission> CreateSubmissionAsync(Submission submission)
        {
            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();
            
            // Reload with navigation properties
            return await GetSubmissionByIdAsync(submission.Id) ?? submission;
        }

        public async Task<Submission> UpdateSubmissionAsync(Submission submission)
        {
            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync();
            return submission;
        }
    }
}
