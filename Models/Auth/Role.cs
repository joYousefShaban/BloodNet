using Microsoft.AspNetCore.Identity;

namespace BloodNet.Models.Auth
{
    public class Role : IdentityRole<Guid>
    {
        public Role() : base() { }
        public Role(string roleName) : base(roleName) { }
    }
}