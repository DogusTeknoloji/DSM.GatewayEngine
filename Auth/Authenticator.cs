using DSM.Core.Ops;
using Nancy;
using System;

namespace DSM.GatewayEngine.Auth
{
    public class Authenticator : NancyModule
    {
        private readonly AuthEngine _authEngine;

        private User UserInfo => new User
        {
            Username = (string)this.Request.Form.Username,
            Password = (string)this.Request.Form.Password
        };

        public Authenticator() : base("/Auth")
        {
            this.Post(WebOperations.WebMethod.POST_AUTH_LOGIN, new Func<dynamic, object>(this.Login));
            this.Post(WebOperations.WebMethod.POST_AUTH_SIGNUP_AGENT, new Func<dynamic, object>(this.SignUp));

            this.Delete(WebOperations.WebMethod.DELETE_AUTH_DESTROY_API, new Func<dynamic, object>(this.DestroyApi));
        }

        private object Login(dynamic arg)
        {
            AuthEngine engine = new AuthEngine(this.UserInfo.Username, this.UserInfo.Password);
            engine.SignUpIfDoesntExist();

            string token = engine.AcquireToken();

            if (string.IsNullOrEmpty(token)) return new Response { StatusCode = HttpStatusCode.Unauthorized };

            Core.Models.AuthServices.SignedUser loginInfo = engine.SignInWithToken(token);
            if (loginInfo == null) return new Response { StatusCode = HttpStatusCode.Unauthorized };

            return this.Response.AsJson(loginInfo);
        }

        private object SignUp(dynamic arg)
        {
            AuthEngine engine = new AuthEngine(this.UserInfo.Username, this.UserInfo.Password);
            bool result = engine.SignUp();
            return result;
        }

        private object DestroyApi(dynamic arg)
        {
            AuthEngine engine = new AuthEngine(this.UserInfo.Username, this.UserInfo.Password);
            string token = (string)this.Request.Form.ApiKey;
            engine.RemoveToken(token);
            return new Response { StatusCode = HttpStatusCode.OK };
        }
    }
}
