using System.ComponentModel.DataAnnotations;

namespace Service.DTO
{
    public class CreateSubjectDto
    {
        [Required(ErrorMessage = "Subject code is required")]
        [StringLength(50, ErrorMessage = "Subject code cannot exceed 50 characters")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject name is required")]
        [StringLength(200, ErrorMessage = "Subject name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;
    }
}
