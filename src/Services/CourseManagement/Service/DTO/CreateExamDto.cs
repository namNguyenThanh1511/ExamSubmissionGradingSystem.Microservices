using System.ComponentModel.DataAnnotations;

namespace Service.DTO
{
    public class CreateExamDto
    {
        [Required(ErrorMessage = "Subject ID is required")]
        public long SubjectId { get; set; }

        [Required(ErrorMessage = "Semester ID is required")]
        public long SemesterId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public DateTime EndTime { get; set; }
    }
}
