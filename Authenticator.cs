using DSM.Core.Models.AuthServices;
using DSM.Core.Ops;
using DSM.OrmService;
using Nancy;
using System;
using System.Security.Claims;

namespace DSM.GatewayEngine
{
    public class Authenticator : NancyModule
    {

        private static readonly GatewayAutheticationRepository _gatewayAutheticationRepository = new GatewayAutheticationRepository();

        public Authenticator() : base("/auth")
        {
            Get(WebOperations.WebMethod.GET_AUTH_IS_VALID, new Func<dynamic, object>(IsValid));
            //Get[WebOperations.WebMethod.GET_AUTH_IS_VALID] = new Func<dynamic, dynamic>(IsValid);

            // Post Login method is used mainly to fetch the api key for subsequent calls.
            Post(WebOperations.WebMethod.POST_AUTH_LOGIN, new Func<dynamic, object>(Login));
            //Post[WebOperations.WebMethod.POST_AUTH_LOGIN] = new Func<dynamic, dynamic>(Login);
            Post(WebOperations.WebMethod.POST_AUTH_SIGNUP_AGENT, new Func<dynamic, object>(SignUpAgent));
            //Post[WebOperations.WebMethod.POST_AUTH_SIGNUP_AGENT] = new Func<dynamic, dynamic>(SignUpAgent);

            // Destroy the api key.
            Delete(WebOperations.WebMethod.DELETE_AUTH_DESTROY_API, new Func<dynamic, object>(DestroyApi));
            //Delete[WebOperations.WebMethod.DELETE_AUTH_DESTROY_API] = new Func<dynamic, dynamic>(DestroyApi);
        }

        private static bool SignUpAgentService(string username, string password)
        {
            bool result = _gatewayAutheticationRepository.SignUpAgent(username, password);
            Gateway._logManager.Write(string.Format("Searching the device on Database {0}", result ? "Unknown Device" : "Known Device"));
            return result;
        }
        private static bool IsValidUser(string username, string password)
        {
            bool result = _gatewayAutheticationRepository.IsValidUser(username, password);
            Gateway._logManager.Write(string.Format("User Validation -> {0}!", result ? "Success" : "Failed"));
            return result;
        }
        private static string AcquireApiKey(string username, string password)
        {
            string apiKey = _gatewayAutheticationRepository.ObtainApiKey(username, password);
            Gateway._logManager.Write($"Acquiring ApiKey -> Success! ({apiKey})");
            return apiKey;
        }
        private static void RemoveApiKey(string apiKey)
        {
            _gatewayAutheticationRepository.RemoveApiKey(apiKey);
        }

        private object IsValid(dynamic arg)
        {
            string userName = (string)Request.Form.Username;
            string password = (string)Request.Form.Password;

            bool res = IsValidUser(userName, password);
            return res;
        }
        private object Login(dynamic arg)
        {
            string userName = (string)Request.Form.Username;
            string password = (string)Request.Form.Password;
            Gateway._logManager.Write($"Reading Device Info -> Success! ({userName}/{password})");
            switch (password)
            {
                case "DSM.Agents.MonitorService":
                case "DSM.Agents.TrackerService":
                case "DSM.Agents.LogService":
                case "DSM.Agents.PlatformService":
                case "DSM.Agents.PostOfficeService":
                case "TESTINF":
                    if (!IsValidUser(userName, password))
                    {
                        SignUpAgentService(userName, password);
                    }

                    break;
                default: break;
            }

            string apiKey = AcquireApiKey(userName, password);

            return string.IsNullOrEmpty(apiKey)
                            ? new Response { StatusCode = HttpStatusCode.Unauthorized }
                            : Response.AsJson(new { ApiKey = apiKey });
        }
        private object SignUpAgent(dynamic arg)
        {
            string userName = (string)Request.Form.Username;
            string password = (string)Request.Form.Password;

            bool res = SignUpAgentService(userName, password);
            return res;
        }
        private object DestroyApi(dynamic arg)
        {
            string apiKey = (string)Request.Form.ApiKey;
            RemoveApiKey(apiKey);
            return new Response { StatusCode = HttpStatusCode.OK };
        }

        internal static ClaimsPrincipal SignInWithApiKey(string apikey)
        {

            SignedUser user = _gatewayAutheticationRepository.SignWithApiKey(apikey);
            if (user == null)
            {
                return null;
            }

            return new SignedClient(user.UserName, user.Role);
        }
    }
}