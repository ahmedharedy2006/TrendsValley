using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TrendsValley.Models.Models;

namespace TrendsValley.Services
{
    public class RoleClaimsTransformation : IClaimsTransformation
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleClaimsTransformation(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = (ClaimsIdentity)principal.Identity!;
            var user = await _userManager.GetUserAsync(principal);
            if (user == null) return principal;

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null) continue;

                var roleClaims = await _roleManager.GetClaimsAsync(role);
                foreach (var claim in roleClaims)
                {
                    if (!principal.HasClaim(claim.Type, claim.Value))
                    {
                        identity.AddClaim(claim);
                    }
                }
            }

            return principal;
        }
    }
}
