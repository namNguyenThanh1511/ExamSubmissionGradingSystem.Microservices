using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;

namespace Repository.Repository
{
    public class CriterionScoreRepository : ICriterionScoreRepository
    {
        private readonly ExamManagementContext _context;

        public CriterionScoreRepository(ExamManagementContext context)
        {
            _context = context;
        }

        public async Task<CriterionScore?> GetCriterionScoreAsync(long submissionId, long rubricCriterionId)
        {
            return await _context.CriterionScores
                .FirstOrDefaultAsync(cs => cs.SubmissionId == submissionId && cs.RubricCriterionId == rubricCriterionId);
        }

        public async Task<CriterionScore> CreateOrUpdateCriterionScoreAsync(CriterionScore criterionScore)
        {
            var existing = await GetCriterionScoreAsync(criterionScore.SubmissionId, criterionScore.RubricCriterionId);
            
            if (existing != null)
            {
                // Update existing
                existing.Score = criterionScore.Score;
                existing.Comment = criterionScore.Comment;
                _context.CriterionScores.Update(existing);
                await _context.SaveChangesAsync();
                return existing;
            }
            else
            {
                // Create new
                _context.CriterionScores.Add(criterionScore);
                await _context.SaveChangesAsync();
                return criterionScore;
            }
        }

        public async Task<IEnumerable<CriterionScore>> GetCriterionScoresBySubmissionIdAsync(long submissionId)
        {
            return await _context.CriterionScores
                .Include(cs => cs.RubricCriterion)
                .Where(cs => cs.SubmissionId == submissionId)
                .ToListAsync();
        }

        public async Task DeleteCriterionScoreAsync(long id)
        {
            var criterionScore = await _context.CriterionScores.FindAsync(id);
            if (criterionScore != null)
            {
                _context.CriterionScores.Remove(criterionScore);
                await _context.SaveChangesAsync();
            }
        }
    }
}

