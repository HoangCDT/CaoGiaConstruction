using System.Security.Claims;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.WebClient.Const;

namespace CaoGiaConstruction.WebClient.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
            {
                return 0;
            }

            return user.FindFirst(ClaimTypes.NameIdentifier).Value.ToInt();
        }

        public static string GetValueByType(this ClaimsPrincipal user, string type)
        {
            var userClaims = user.Claims.FirstOrDefault(x => x.Type == type);
            if (userClaims != null)
            {
                return userClaims.Value.ToSafetyString();
            }
            return string.Empty;
        }

        public static bool HasAdminOrStaffRole(this ClaimsPrincipal user)
        {
            var role = user.GetValueByType("rolefirst");
            return role == RoleConst.ADMIN || role == RoleConst.STAFF;
        }
    }
}