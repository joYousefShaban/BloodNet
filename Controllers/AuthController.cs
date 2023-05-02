using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetUserInfo()
        {
            IActionResult result = NotFound();
            var identity = HttpContext.User.Claims as IEnumerable<Claim>;
            if (identity.Any())
            {
                var name = identity.FirstOrDefault(x => x.Type.Equals("Name"))?.Value ?? string.Empty;
                var anything = identity.FirstOrDefault(x => x.Type.Equals("TestClaim"))?.Value ?? string.Empty;

                result = Ok(new { name, text = anything });
            }
            return result;
        }
    }
}