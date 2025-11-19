namespace Service.DTO
{
    public class ViolationDto
    {
        public long Id { get; set; }
        public long? SubmissionId { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public bool? Verified { get; set; }
        public string? StudentCode { get; set; }
        public string? ExamTitle { get; set; }
    }
}
