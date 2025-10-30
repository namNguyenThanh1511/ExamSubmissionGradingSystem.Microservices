using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Shared.Extension;

namespace IAM.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            // ... logic

            //if (!valid)
            //    return this.ToErrorResponse("Login failed", "Invalid username or password");

            var token = "...";
            return this.ToApiResponse(new { token, role = "Admin" }, "Login successful");
        }
    }
}
