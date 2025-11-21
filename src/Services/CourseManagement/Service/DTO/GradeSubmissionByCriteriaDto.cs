using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Service.DTO
{
    public class GradeSubmissionByCriteriaDto
    {
        [Required(ErrorMessage = "Submission ID is required")]
        [JsonPropertyName("submissionId")]
        public long SubmissionId { get; set; }

        [Required(ErrorMessage = "Scores are required")]
        [MinLength(1, ErrorMessage = "At least one score is required")]
        [JsonPropertyName("scores")]
        public List<CriterionScoreDto> Scores { get; set; } = new List<CriterionScoreDto>();
    }

    public class CriterionScoreDto
    {
        [Required(ErrorMessage = "Criterion ID is required")]
        [JsonPropertyName("criterionId")]
        public long CriterionId { get; set; }

        [Required(ErrorMessage = "Score is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Score must be greater than or equal to 0")]
        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
    }
}

