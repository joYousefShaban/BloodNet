using BloodNet.Models.Auth;
using BloodNet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BloodNet.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace BloodNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly BloodNetContext _context;

        public RegisterController(IConfiguration config, SignInManager<User> signInManager, UserManager<User> userManager, BloodNetContext context)
        {
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserLoginModel model)
        {
            IActionResult result = Unauthorized();
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email
            };
            //check email already
            var userWithEmail = await _userManager.FindByEmailAsync(model.Email);
            if (userWithEmail == null)
            {
                var createUserResult = await _userManager.CreateAsync(user, model.Password);
                if (createUserResult.Succeeded)
                {
                    return StatusCode(201);
                }
                else
                {
                    return Ok(createUserResult.Errors.ToString());
                }
            }
            else
                return Conflict();
        }
    }
}