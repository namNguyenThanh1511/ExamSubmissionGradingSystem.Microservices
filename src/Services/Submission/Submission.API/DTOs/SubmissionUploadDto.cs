using Microsoft.AspNetCore.Mvc;

namespace Submission.API.DTOs
{
    public class SubmissionUploadDto
    {
        [FromForm]
        public IFormFile File { get; set; }
        [FromForm]
        public string Metadata { get; set; }
    }
}
