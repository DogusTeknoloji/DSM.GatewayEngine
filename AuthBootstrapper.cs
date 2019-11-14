using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.TinyIoc;
using System.Security.Claims;

namespace DSM.GatewayEngine
{
    public class AuthBootstrapper : DefaultNancyBootstrapper
    {

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {

            // At request startup we modify the request pipelines to
            // include stateless authentication

            // Configuring stateless authentication is simple.Just use the
            //NancyContext to get the apiKey. Then, use the apiKey to get
            //your user's identity.
            StatelessAuthenticationConfiguration configuration =
                new StatelessAuthenticationConfiguration(nancyContext =>
                {
                    //for now, we will pull the apiKey from the querystring,
                    //but you can pull it from any part of the NancyContext
                    string apiKey = (string)nancyContext.Request.Query.NGAuthVKey.Value;
                    if (apiKey?.Length < 1)
                    {
                        return null;
                    }

                    //get the user identity however you choose to (for now, using a static class/method)
                    return Authenticator.SignInWithApiKey(apiKey) as ClaimsPrincipal;
                });

            AllowAccessToConsumingSite(pipelines);
            StatelessAuthentication.Enable(pipelines, configuration);
        }
        public override void Configure(INancyEnvironment environment)
        {
            environment.Tracing(enabled: false, displayErrorTraces: true);
            base.Configure(environment);
        }

        private static void AllowAccessToConsumingSite(IPipelines pipelines)
        {
            pipelines.AfterRequest.AddItemToEndOfPipeline(x =>
            {
                x.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                x.Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,DELETE,PUT,OPTIONS");
            });
        }
    }
}
