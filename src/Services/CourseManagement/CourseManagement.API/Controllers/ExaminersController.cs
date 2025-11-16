using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Services;
using Shared.Extension;

namespace CourseManagement.API.Controllers
{
    /// <summary>
    /// Examiners Controller
    /// 
    /// Role Mapping (Currently Commented Out):
    /// - Manager: /api/examiners (GET, POST)
    /// </summary>
    [ApiController]
    [Route("api/examiners")]
    public class ExaminersController : ControllerBase
    {
        private readonly IExaminerService _examinerService;
        private readonly ILogger<ExaminersController> _logger;

        public ExaminersController(IExaminerService examinerService, ILogger<ExaminersController> logger)
        {
            _examinerService = examinerService;
            _logger = logger;
        }

        /// <summary>
        /// L?y danh sách giám kh?o
        /// Role: Manager
        /// </summary>
        /// <returns>List of examiners</returns>
        [HttpGet]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllExaminers()
        {
            try
            {
                var examiners = await _examinerService.GetAllExaminersAsync();
                return this.ToApiResponse(examiners, "Examiners retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving examiners");
                return this.ToErrorResponse("Error retrieving examiners", ex.Message);
            }
        }

        /// <summary>
        /// T?o giám kh?o
        /// Role: Manager
        /// </summary>
        /// <param name="createExaminerDto">Examiner data</param>
        /// <returns>Created examiner</returns>
        [HttpPost]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateExaminer([FromBody] CreateExaminerDto createExaminerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray();
                    return this.ToErrorResponse("Invalid data", errors);
                }

                var examiner = await _examinerService.CreateExaminerAsync(createExaminerDto);
                return this.ToApiResponse(examiner, "Examiner created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating examiner");
                return this.ToErrorResponse("Error creating examiner", ex.Message);
            }
        }
    }
}
