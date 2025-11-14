namespace Submission.Services.DTOs
{
    public class StudentSubmissionDto
    {
        public Guid Id { get; set; }
        public string StudentId { get; set; }
        public string SolutionUrl { get; set; }
        public bool IsValid { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DateTime UploadAt { get; set; }
    }
}
