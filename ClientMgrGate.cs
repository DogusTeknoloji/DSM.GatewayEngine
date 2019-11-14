using DSM.Core.Models.Management;
using DSM.Core.Ops;
using DSM.Core.Ops.ConsoleTheming;
using DSM.OrmService;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DSM.GatewayEngine
{
    public class ClientMgrGate : NancyModule
    {

        private readonly ClientManagerRepository _clientManagerRepository = new ClientManagerRepository();

        public ClientMgrGate() : base(@"DSM.GatewayEngine\ClientMgrGate")
        {
            XConsole.SetDefaultColorSet(ConsoleColorSetGreen.Instance);

            Get(WebOperations.WebMethod.GET_CLIENT_LIST, action: new Func<dynamic, object>(GetClientList));
            Get(WebOperations.WebMethod.GET_CLIENT_BY_MACHINE_NAME.WithParamMachineName(), new Func<dynamic, object>(GetClient));
            Get(WebOperations.WebMethod.GET_SCHEDULER_BY_CLIENTID_AND_SERVICEID, new Func<dynamic, object>(GetScheduler));

            Post(WebOperations.WebMethod.POST_CLIENT, new Func<dynamic, object>(PostClient));
            Post(WebOperations.WebMethod.POST_SCHEDULER, new Func<dynamic, object>(SetScheduler));
            Post(WebOperations.WebMethod.POST_ENABLE_SCHEDULER, new Func<dynamic, object>(EnableScheduler));
            Post(WebOperations.WebMethod.POST_DISABLE_SCHEDULER, new Func<dynamic, object>(DisableScheduler));
        }

        private object GetClient(dynamic parameters)
        {
            Client client = this._clientManagerRepository.GetClient(parameters.Id);
            return Response.AsJson(client);
        }
        private object GetClientList(dynamic parameters)
        {
            List<Client> clients = _clientManagerRepository.All().ToList();
            return Response.AsJson(clients);
        }
        private object PostClient(dynamic parameters)
        {
            Client client = this.Bind<Client>();
            int result = _clientManagerRepository.SaveClient(client);
            return result;
        }
        private object GetScheduler(dynamic parameters)
        {
            ServiceTimer timer = this._clientManagerRepository.GetScheduler(parameters.ClientId, parameters.ServiceId);
            return Response.AsJson(timer);
        }
        private object SetScheduler(dynamic parameters)
        {
            ServiceTimer timer = this.Bind<ServiceTimer>();
            int result = _clientManagerRepository.SetScheduler(timer);
            return Response.AsJson(result);
        }
        private object EnableScheduler(dynamic parameters)
        {
            ServiceTimer timer = this.Bind<ServiceTimer>();
            int result = this._clientManagerRepository.EnableScheduler(parameters.ServiceId, parameters.ClientId);
            return Response.AsJson(result);
        }
        private object DisableScheduler(dynamic parameters)
        {
            ServiceTimer timer = this.Bind<ServiceTimer>();
            int result = this._clientManagerRepository.DisableScheduler(parameters.ServiceId, parameters.ClientId);
            return Response.AsJson(result);
        }
    }
}

