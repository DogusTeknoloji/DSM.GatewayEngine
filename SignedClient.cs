using System.Security.Claims;

namespace DSM.GatewayEngine
{
    public class SignedClient : ClaimsPrincipal
    {
        public SignedClient(string Username, string claim)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity("Stateless");
            AddIdentity(claimsIdentity);
        }
    }
}
