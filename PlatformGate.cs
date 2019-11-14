using DSM.Core.Models.PlaftormServices;
using DSM.Core.Ops;
using DSM.Core.Ops.ConsoleTheming;
using DSM.OrmService.PlatformServices;
using Nancy;
using Nancy.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace DSM.GatewayEngine
{
    public class PlatformGate : SecureModule
    {
        private readonly ADDomainUserRepository _domainUserRepository = new ADDomainUserRepository();
        private readonly ADDomainRepository _domainRepository = new ADDomainRepository();
        public PlatformGate() : base(@"DSM.GatewayEngine\PlatformGate")
        {
            XConsole.SetDefaultColorSet(ConsoleColorSetGreen.Instance);

            Get(WebOperations.WebMethod.GET_ADDOMAIN_LIST, new System.Func<dynamic, object>(GetAllDomainUsers));
            Get(WebOperations.WebMethod.GET_ADDOMAIN_LIST, new System.Func<dynamic, object>(GetDomains));
            Post(WebOperations.WebMethod.POST_ADDOMAIN_USER, new System.Func<dynamic, object>(PostDomainUser));
            Post(WebOperations.WebMethod.POST_ADDOMAIN_USER_MULTIPLE, new System.Func<dynamic, object>(PostDomainUserMulti));
        }

        private object GetAllDomainUsers(dynamic parameters)
        {
            IEnumerable<ADDomainUser> domainUsers = _domainUserRepository.All();
            logManager.Write($"[GET]-> GetAllDomainUsers Count: {domainUsers.Count()}");
            return Response.AsJson(domainUsers);
        }

        private object PostDomainUser(dynamic parameters)
        {
            ADDomainUser domainUser = this.Bind<ADDomainUser>();
            int result = _domainUserRepository.SaveDomainUser(domainUser);
            logManager.Write($"[POST]-> PostDomainUser Id: {result}");
            return Response.AsJson(result);
        }

        private object PostDomainUserMulti(dynamic arg)
        {
            ADDomainUser[] domainUser = this.Bind<ADDomainUser[]>();
            IEnumerable<ADDomainUser> result = _domainUserRepository.SaveDomainUser(domainUser);
            logManager.Write($"[POST]-> PostDomainUser Id: {result}");
            return Response.AsJson(result);
        }

        private object GetDomains(dynamic parameters)
        {
            IEnumerable<ADDomain> domains = _domainRepository.All();
            logManager.Write($"[GET]-> GetAllDomains Count:{domains.Count()}");
            return Response.AsJson(domains);
        }
    }
}