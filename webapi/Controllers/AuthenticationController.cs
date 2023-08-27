using Business.Security;
using Data.VMs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IAuthenticationService _authService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authService = authenticationService;
        }
        [HttpGet]
        [Route("/Authentication/GetUserData/{UserName}/{Password}")]

        public async Task<GetUserDataResponse> GetUserData(string UserName, string Password)
        {
            return await _authService.IsAuthenticated(UserName, Password);
        }
    }
}
