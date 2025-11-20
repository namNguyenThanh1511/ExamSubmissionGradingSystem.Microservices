using System;

namespace Service.DTO
{
    public class SubmissionDto
    {
        public long Id { get; set; }
        public long? ExamId { get; set; }
        public Guid? SubmissionId { get; set; }
        public double? TotalScore { get; set; }
        public long? ExaminerId { get; set; }
        public string? ExaminerName { get; set; }
    }
}
