using DSM.Core.Ops;
using Nancy;
using Nancy.ErrorHandling;
using System.IO;

namespace DSM.GatewayEngine
{
    public class StatusCodeHandler : IStatusCodeHandler
    {
        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            context.Response.Contents = stream =>
            {
                string executingRoot = FileOperations.AssemblyDirectory;
                string fileName = string.Empty;
                switch (statusCode)
                {
                    default: break;
                    case HttpStatusCode.NotFound:
                        fileName = string.Concat(executingRoot, "\\NGIndex\\404.html"); break;
                    case HttpStatusCode.Unauthorized:
                        fileName = string.Concat(executingRoot, "\\NGIndex\\401.html"); break;
                }
                using (FileStream file = File.OpenRead(fileName))
                {
                    file.CopyTo(stream);
                }
            };
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound
                || statusCode == HttpStatusCode.Unauthorized;
        }
    }
}
