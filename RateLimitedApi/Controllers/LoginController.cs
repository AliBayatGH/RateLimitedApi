using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;

namespace RateLimitedApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IConfiguration configuration,ILogger<LoginController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> Login()
        {
            //await Task.Delay(1000);
            // Retrieve client IP address
            string clientIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            _logger.LogInformation($"Login request received from IP: {clientIpAddress}");

            return Ok("Login successful");
        }
    }
}
