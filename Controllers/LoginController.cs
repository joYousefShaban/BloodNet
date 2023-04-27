﻿using BloodNet.Models;
using BloodNet.Models.Auth;
using BloodNet.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BloodNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly BloodNetContext _context;

        public LoginController(IConfiguration config, SignInManager<User> signInManager, UserManager<User> userManager, BloodNetContext context)
        {
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginModel login)
        {
            IActionResult result = Unauthorized();
            var user = await AuthenticateUser(login);
            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                result = Ok(new { token = tokenString });
            }
            return result;
        }

        private object GenerateJSONWebToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:key"]));
            var credentitials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claimsIdentity = new[]
            {
                new Claim("Name",user.Email),
                new Claim("TestClaim","Anything you want (Token is valid)")
            };


            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims: claimsIdentity,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentitials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        private async Task<User> AuthenticateUser(UserLoginModel login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var userInfo = await _userManager.FindByEmailAsync(login.Email);
                return userInfo;
            }
            return null;
        }
    }
}
