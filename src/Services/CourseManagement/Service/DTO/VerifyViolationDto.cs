using System.ComponentModel.DataAnnotations;

namespace Service.DTO
{
    public class VerifyViolationDto
    {
        [Required(ErrorMessage = "Verified status is required")]
        public bool Verified { get; set; }
    }
}
