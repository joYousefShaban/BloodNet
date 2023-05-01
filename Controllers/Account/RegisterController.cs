using BloodNet.Models.Auth;
using BloodNet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BloodNet.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using BloodNet.Services.EmailService;

namespace BloodNet.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly BloodNetContext _context;
        private readonly IEmailService _emailService;

        public RegisterController(IConfiguration config, SignInManager<User> signInManager, UserManager<User> userManager, BloodNetContext context, IEmailService emailService)
        {
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
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

                //add a user to a role
                await _userManager.AddToRoleAsync(user, "Admin");
                if (createUserResult.Succeeded)
                {
                    var returnUrl = Url.Content("~/");
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    _emailService.SendRegisterEmail(new EmailDTO { To = model.Email, callbackUrl = callbackUrl });


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