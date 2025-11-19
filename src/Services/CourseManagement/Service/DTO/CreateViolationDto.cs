using System.ComponentModel.DataAnnotations;

namespace Service.DTO
{
    public class CreateViolationDto
    {
        [Required(ErrorMessage = "Submission ID is required")]
        public long SubmissionId { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [StringLength(100, ErrorMessage = "Type cannot exceed 100 characters")]
        public string Type { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }
    }
}
