using System.ComponentModel.DataAnnotations;

namespace Service.DTO
{
    public class GradeSubmissionDto
    {
        [Required(ErrorMessage = "Total score is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Total score must be greater than or equal to 0")]
        public double TotalScore { get; set; }
    }
}
