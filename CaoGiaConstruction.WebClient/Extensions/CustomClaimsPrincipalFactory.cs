using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace CaoGiaConstruction.WebClient.Extensions
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, Role>
    {
        private readonly UserManager<User> _userManger;

        public CustomClaimsPrincipalFactory(UserManager<User> userManager,
            RoleManager<Role> roleManager, IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
            _userManger = userManager;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(User user)
        {
            var principal = await base.CreateAsync(user);
            var roles = await _userManger.GetRolesAsync(user);
            string role = string.Empty;
            if (roles.Any())
            {
                role = roles.FirstOrDefault();
            }
            ((ClaimsIdentity)principal.Identity).AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserName),
                 new Claim(ClaimTypes.Email, user.Email??string.Empty),
                       new Claim(ClaimTypes.Role,role),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypeConst.ID, user.Id.ToString()),
                            new Claim(ClaimTypeConst.USERNAME, user.UserName??string.Empty),
                            new Claim(ClaimTypeConst.NAME, user.FullName??string.Empty),
                            new Claim(ClaimTypeConst.ADDRESS, user.Address??string.Empty),
                            new Claim(ClaimTypeConst.OPENDATE, user.CreatedDate.ToString("dd/MM/yyyy")),
                            new Claim(ClaimTypeConst.EMAIL, user.Email??string.Empty),
                            new Claim(ClaimTypeConst.AVATAR,user.Avatar??string.Empty),
                            new Claim(ClaimTypeConst.PHONENUMBER, user.PhoneNumber??string.Empty),
                            new Claim(ClaimTypeConst.ROLES,roles.ToJsonString()),
                            new Claim(ClaimTypeConst.ROLE_FIRST,role)
            });
            return principal;
        }
    }
}