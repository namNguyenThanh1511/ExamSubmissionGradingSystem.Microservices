using Repository.Models;

namespace Repository.Repository
{
    public interface ICriterionScoreRepository
    {
        Task<CriterionScore?> GetCriterionScoreAsync(long submissionId, long rubricCriterionId);
        Task<CriterionScore> CreateOrUpdateCriterionScoreAsync(CriterionScore criterionScore);
        Task<IEnumerable<CriterionScore>> GetCriterionScoresBySubmissionIdAsync(long submissionId);
        Task DeleteCriterionScoreAsync(long id);
    }
}

