using System.ComponentModel.DataAnnotations;

namespace Service.DTO
{
    public class CreateRubricDto
    {
        [Required(ErrorMessage = "Rubric name is required")]
        [StringLength(255, ErrorMessage = "Rubric name cannot exceed 255 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Exam ID is required")]
        public long ExamId { get; set; }

        [Required(ErrorMessage = "At least one criterion is required")]
        [MinLength(1, ErrorMessage = "At least one criterion is required")]
        public List<CreateRubricCriterionDto> RubricCriteria { get; set; } = new List<CreateRubricCriterionDto>();
    }
}
