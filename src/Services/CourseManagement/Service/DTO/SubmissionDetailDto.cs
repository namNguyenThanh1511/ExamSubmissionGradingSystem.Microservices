using System.Text.Json.Serialization;

namespace Service.DTO
{
    public class SubmissionDetailDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("examId")]
        public long? ExamId { get; set; }

        [JsonPropertyName("studentCode")]
        public string StudentCode { get; set; } = string.Empty;

        [JsonPropertyName("submissionId")]
        public Guid? SubmissionId { get; set; }

        [JsonPropertyName("totalScore")]
        public double? TotalScore { get; set; }

        [JsonPropertyName("examinerId")]
        public long? ExaminerId { get; set; }

        [JsonPropertyName("examinerName")]
        public string? ExaminerName { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("criterionScores")]
        public List<CriterionScoreDetailDto> CriterionScores { get; set; } = new List<CriterionScoreDetailDto>();
    }

    public class CriterionScoreDetailDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("criterionId")]
        public long CriterionId { get; set; }

        [JsonPropertyName("criterionName")]
        public string? CriterionName { get; set; }

        [JsonPropertyName("maxScore")]
        public double? MaxScore { get; set; }

        [JsonPropertyName("score")]
        public double? Score { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
    }
}

