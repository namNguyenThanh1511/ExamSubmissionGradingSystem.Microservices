using System.ComponentModel.DataAnnotations;

namespace Service.DTO
{
    public class CreateRubricCriterionDto
    {
        [Required(ErrorMessage = "Criterion name is required")]
        [StringLength(255, ErrorMessage = "Criterion name cannot exceed 255 characters")]
        public string CriterionName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Max score is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Max score must be a positive number")]
        public double MaxScore { get; set; }
    }
}
