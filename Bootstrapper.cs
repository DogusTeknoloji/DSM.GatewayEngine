using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.TinyIoc;
using System.Security.Claims;

namespace DSM.GatewayEngine
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            StatelessAuthenticationConfiguration configuration =
                new StatelessAuthenticationConfiguration(nancyContext =>
                {
                    string token = (string)nancyContext.Request.Headers.Authorization;
                    if (token?.Length < 1) return null;

                    return Authenticator.SignInWithApiKey(token) as ClaimsPrincipal;
                });

            ConfigureOrigin(pipelines);
            StatelessAuthentication.Enable(pipelines, configuration);
        }

        public override void Configure(INancyEnvironment environment)
        {
            environment.Tracing(enabled: false, displayErrorTraces: true);
            base.Configure(environment);
        }

        /// <summary>
        /// Allow Origin Access
        /// </summary>
        /// <param name="pipelines">Request Pipeline</param>
        private static void ConfigureOrigin(IPipelines pipelines)
        {
            pipelines.AfterRequest.AddItemToEndOfPipeline(x =>
            {
                x.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                x.Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,DELETE,PUT,OPTIONS");
            });
        }
    }
}
