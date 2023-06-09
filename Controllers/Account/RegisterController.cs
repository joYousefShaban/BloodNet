﻿using BloodNet.Models;
using BloodNet.Models.Auth;
using BloodNet.Models.ViewModel;
using BloodNet.Services.EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace BloodNet.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public RegisterController(UserManager<User> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserLoginModel model)
        {
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
                        values: new { area = "Identity", userId, code, returnUrl },
                        protocol: Request.Scheme) ?? "";

                    _emailService.SendRegisterEmail(new EmailDTO { To = model.Email, CallbackUrl = callbackUrl });


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