using DSM.Core.Ops;
using Nancy;
using Nancy.Security;

namespace DSM.GatewayEngine
{
    public abstract class SecureModule : NancyModule
    {
        protected LogManager logManager;
        public SecureModule(string logPath)
        {
            this.RequiresAuthentication();
            logManager = LogManager.GetManager(logPath);
        }
    }
}
