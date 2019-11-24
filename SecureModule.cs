using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        public SecureModule(string logPath,string urlPath) : base(urlPath)
        {
            this.RequiresAuthentication();
         logManager =LogManager.GetManager(logPath);
        }

        protected bool ValidateRoles(params string[]roles)
        {
            try
            {
                List<Predicate<Claim>> predicates =  new List<Predicate<Claim>>();
                List<string> roleList=  roles.ToList();
                roleList.ForEach(role=> predicates.Add(claim => claim.Value == role));
                this.RequiresClaims(predicates.ToArray());
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
