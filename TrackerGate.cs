using DSM.Core.Interfaces.AppServices;
using DSM.Core.Models;
using DSM.Core.Ops;
using DSM.Core.Ops.ConsoleTheming;
using DSM.OrmService;
using DSM.OrmService.AppServices;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DSM.GatewayEngine
{
    public class TrackerGate : SecureModule
    {
        private readonly SiteTrackerRepository _siteTrackerRepository = new SiteTrackerRepository();
        private readonly SiteEndpointRepository _endpointRepository = new SiteEndpointRepository();
        private readonly SiteConnectionStringRepository _connectionStringRepository = new SiteConnectionStringRepository();

        public TrackerGate() : base(@"DSM.GatewayEngine\TrackerGate")
        {
            XConsole.SetDefaultColorSet(ConsoleColorSetGreen.Instance);


            Get(WebOperations.WebMethod.GET_ENDPOINTS_BY_SITEID.WithParamSiteId(), new Func<dynamic, object>(GetEndpointByFilter));
            Get(WebOperations.WebMethod.GET_CONNECTION_STRINGS_BY_SITEID.WithParamSiteId(), new Func<dynamic, object>(GetConnectionStringByFilter));

            Get(WebOperations.WebMethod.GET_ENDPOINT_BY_SITEID.WithParamId(), new Func<dynamic, object>(GetEndpoint));
            Get(WebOperations.WebMethod.GET_CONNECTION_STRING_BY_SITEID.WithParamId(), new Func<dynamic, object>(GetConnectionString));

            Get(WebOperations.WebMethod.GET_ENDPOINT_ALL, new Func<dynamic, object>(GetEndpointAll));

            Post(WebOperations.WebMethod.POST_ENDPOINT, new Func<dynamic, object>(PostEndpoint));
            Post(WebOperations.WebMethod.POST_ENDPOINT_MULTIPLE, new Func<dynamic, object>(PostEndpointMulti));

            Post(WebOperations.WebMethod.POST_CONNECTION_STRING, new Func<dynamic, object>(PostConnectionString));
            Post(WebOperations.WebMethod.POST_CONNECTION_STRING_MULTIPLE, new Func<dynamic, object>(PostConnectionStringMulti));

            Post(WebOperations.WebMethod.POST_SITE_TRACKER, new Func<dynamic, object>(PostSiteTracker));
            Post(WebOperations.WebMethod.POST_SITE, new Func<dynamic, object>(PostSiteTrackerMulti));
        }

        #region SiteEndpointOps
        private dynamic GetEndpointAll(dynamic parameters)
        {
            IEnumerable<ISiteEndpoint> endpointList = _endpointRepository.All();
            logManager.Write($"[GET]-> GetEndpointAll Count: {endpointList.Count()}");
            return Response.AsJson(endpointList);
        }
        private dynamic GetEndpointByFilter(dynamic parameters)
        {
            IEnumerable<ISiteEndpoint> endpointList = this._endpointRepository.GetEndpointList(parameters.SiteId);
            logManager.Write($"[GET]-> GetEndpointByFilter Count: {endpointList.Count()}");
            return Response.AsJson(endpointList);
        }
        private dynamic GetEndpoint(dynamic parameters)
        {
            ISiteEndpoint endpoint = this._endpointRepository.GetSiteEndpoint(parameters.SiteId);
            logManager.Write($"[GET]-> GetEndpoint Id: {endpoint?.Id}");
            return Response.AsJson(endpoint);
        }
        private dynamic PostEndpoint(dynamic parameters)
        {
            SiteEndpoint newEndpoint = this.Bind<SiteEndpoint>();
            int result = _endpointRepository.PostSiteEndpoint(newEndpoint);
            logManager.Write($"[POST]-> PostEndpoint Id: {result}");
            return Response.AsJson(result);
        }
        private object PostEndpointMulti(dynamic arg)
        {
            SiteEndpoint[] newEndpoint = this.Bind<SiteEndpoint[]>();
            IEnumerable<SiteEndpoint> result = _endpointRepository.PostSiteEndpoint(newEndpoint);
            logManager.Write($"[POST]-> PostEndpoint Id: {result}");
            return Response.AsJson(result);
        }
        #endregion

        #region SiteConnectionStringOps
        private dynamic GetConnectionStringByFilter(dynamic parameters)
        {
            IEnumerable<ISiteConnectionString> connectionStringList = this._connectionStringRepository.GetConnectionStringList(parameters.SiteId);
            logManager.Write($"[GET]-> GetConnectionStringByFilter Count: {connectionStringList.Count()}");
            return Response.AsJson(connectionStringList);
        }
        private dynamic GetConnectionString(dynamic parameters)
        {
            ISiteConnectionString connectionString = this._connectionStringRepository.GetSiteConnectionString(parameters.SiteId);
            logManager.Write($"[GET]-> GetConnectionString Id: {connectionString.Id}");
            return Response.AsJson(connectionString);
        }
        private dynamic PostConnectionString(dynamic parameters)
        {
            SiteConnectionString newConnectionString = this.Bind<SiteConnectionString>();
            int result = _connectionStringRepository.PostSiteConnectionString(newConnectionString);
            logManager.Write($"[POST]-> PostConnectionString Id: {result}");
            return Response.AsJson(result);
        }

        private object PostConnectionStringMulti(dynamic arg)
        {
            SiteConnectionString[] newConnectionString = this.Bind<SiteConnectionString[]>();
            IEnumerable<SiteConnectionString> result = _connectionStringRepository.PostSiteConnectionString(newConnectionString);
            logManager.Write($"[POST]-> PostConnectionString Id: {result}");
            return Response.AsJson(result);
        }
        #endregion

        private dynamic PostSiteTracker(dynamic parameters)
        {
            ISiteTracker site = this.Bind<ISiteTracker>();
            int result = _siteTrackerRepository.PostSiteTracker(site);
            logManager.Write($"[POST]-> PostSiteTracker Id:{result}");
            return Response.AsJson(result);
        }

        private object PostSiteTrackerMulti(dynamic arg)
        {
            ISiteTracker[] site = this.Bind<ISiteTracker[]>();
            IEnumerable<ISiteTracker> result = _siteTrackerRepository.PostSiteTracker(site);
            logManager.Write($"[POST]-> PostSiteTracker Id:{result}");
            return Response.AsJson(result);
        }

    }
}
