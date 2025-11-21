namespace Service.DTO
{
    public class SubmissionDto
    {
        public long Id { get; set; }
        public long? ExamId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid? SubmissionId { get; set; }
        public double? TotalScore { get; set; }
        public long? ExaminerId { get; set; }
        public string? ExaminerName { get; set; }
    }
}
