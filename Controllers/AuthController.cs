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
        [Authorize]
        [HttpGet]
        public IActionResult GetUserInfo()
        {
            IActionResult result = NotFound();
            var identity = HttpContext.User.Claims as IEnumerable<Claim>;
            if (identity.Count() > 0)
            {
                var name = identity.FirstOrDefault(x => x.Type.Equals("Name")).Value;
                var anything = identity.FirstOrDefault(x => x.Type.Equals("TestClaim")).Value;

                result = Ok(new { name = name, text = anything });
            }
            return result;
        }
    }
}
