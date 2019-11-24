using DSM.Core.Models.AuthServices;
using DSM.OrmService;
using System.Collections.Generic;

namespace DSM.GatewayEngine.Auth
{
    public class AuthEngine
    {
        private readonly GatewayAutheticationRepository _gatewayAuthenticationRepository = new GatewayAutheticationRepository();
        private readonly string _username;
        private readonly string _password;
        private readonly string _token;
        private readonly List<string> _reservedNames = new List<string>
        {
            "DSM.Agents.MonitorService",
            "DSM.Agents.TrackerService",
            "DSM.Agents.LoggingService",
            "DSM.Agents.PlatformService",
            "DSM.Agents.PostOfficeService",
            "TESTINF"
        };

        public AuthEngine(string username, string password)
        {
            this._username = username;
            this._password = password;
        }
        public AuthEngine(string token)
        {
            this._token = token;
        }
        private bool IsValidUser()
        {
            bool result = this._gatewayAuthenticationRepository.IsValidUser(this._username, this._password);
            return result;
        }
        public bool SignUp()
        {
            if (!this.IsValidUser()) return false;

            bool result = this._gatewayAuthenticationRepository.SignUpAgent(this._username, this._password);
            return result;
        }
        public string AcquireToken()
        {
            if (!this.IsValidUser()) return null;

            string token = this._gatewayAuthenticationRepository.ObtainApiKey(this._username, this._password);
            return token;

        }
        public void RemoveToken(string token)
        {
            this._gatewayAuthenticationRepository.RemoveApiKey(token);
        }
        public void SignUpIfDoesntExist()
        {
            if (!this.IsValidUser() && this._reservedNames.Contains(this._password))
            {
                _ = this.SignUp();
            }
        }
        public SignedUser SignInWithToken(string token)
        {
            SignedUser user = this._gatewayAuthenticationRepository.SignWithApiKey(token);
            if (user == null) return null;

            return user;
        }
        public SignedUser SignInWithToken()
        {
            SignedUser user = this._gatewayAuthenticationRepository.SignWithApiKey(this._token);
            if (user == null) return null;

            return user;
        }
    }
}