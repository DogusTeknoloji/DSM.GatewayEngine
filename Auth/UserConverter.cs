using DSM.Core.Models.AuthServices;
using System.Security.Claims;

namespace DSM.GatewayEngine.Auth
{
    public class UserConverter
    {
        public static ClaimsPrincipal Convert(SignedUser user)
        {
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal();
            ClaimsIdentity claimsIdentity = new ClaimsIdentity("stateless", user.Role, user.Role);
            Claim claim = new Claim("Auth", user.Role);
            claimsIdentity.AddClaim(claim);

            claimsPrincipal.AddIdentity(claimsIdentity);
            return claimsPrincipal;
        }
    }
}
