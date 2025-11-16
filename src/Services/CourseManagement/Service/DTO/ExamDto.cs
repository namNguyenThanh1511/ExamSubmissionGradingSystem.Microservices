namespace Service.DTO
{
    public class ExamDto
    {
        public long Id { get; set; }
        public long? SubjectId { get; set; }
        public long? SemesterId { get; set; }
        public string? Title { get; set; }
        public string? Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        
        // Navigation properties
        public string? SubjectName { get; set; }
        public string? SubjectCode { get; set; }
        public string? SemesterName { get; set; }
    }
}
