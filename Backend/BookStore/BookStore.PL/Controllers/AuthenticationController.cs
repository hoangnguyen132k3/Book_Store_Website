using BookStore.BLL.Services;
using BookStore.BLL.ViewModels.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] RegisterVm register)
        {
            return await _authenticationService.RegisterUserAsync(register);
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromBody] LoginVm loginVm)
        {
            return await _authenticationService.LoginUserAsync(loginVm);
        }

    }
}
