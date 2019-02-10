using System.Threading.Tasks;
using GoalTracker.API.Helpers.Security;
using GoalTracker.Services.Dtos;
using GoalTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GoalTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly Security _security;
        // GET: api/<controller>
        public UserController(IUserService userService,Security security)
        {
            _userService = userService;
            _security = security;
        }


        // POST api/<controller>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserToRegisterDto userToRegister)
        {
            if (!await _userService.FindUserAsync(userToRegister.Name))
            {
                return Ok(await _userService.RegisterUserAsync(userToRegister));
            }
            return BadRequest("Username already exists");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserToLoginDto userToLogin)
        {
            var user = await _userService.LoginUserAsync(userToLogin);

            if (user == null)
                return Unauthorized();

            var token = _security.GenerateToken(user);

            return Ok(token);

        }
       
    }
}
