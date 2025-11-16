using System.ComponentModel.DataAnnotations;

namespace Service.DTO
{
    public class CreateSemesterDto
    {
        [Required(ErrorMessage = "Semester name is required")]
        [StringLength(100, ErrorMessage = "Semester name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateOnly EndDate { get; set; }
    }
}
