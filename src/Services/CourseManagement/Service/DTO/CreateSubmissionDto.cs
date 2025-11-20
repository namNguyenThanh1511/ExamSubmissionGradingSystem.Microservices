using System.ComponentModel.DataAnnotations;

namespace Service.DTO
{
    public class CreateSubmissionDto
    {
        [Required(ErrorMessage = "Exam ID is required")]
        public long ExamId { get; set; }

        [Required(ErrorMessage = "Submission ID is required")]
        public Guid SubmissionId { get; set; }

        [Required(ErrorMessage = "Student Code is required")]
        public string StudentCode { get; set; } = string.Empty;
    }
}

