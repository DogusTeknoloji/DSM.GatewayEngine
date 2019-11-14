using DSM.Core.Models;
using DSM.Core.Ops;
using DSM.Core.Ops.ConsoleTheming;
using DSM.OrmService;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;

namespace DSM.GatewayEngine
{
    public class IISLogGate : SecureModule
    {
        private readonly SiteTransactionRepository _siteTransactionRepository = new SiteTransactionRepository();

        public IISLogGate() : base(@"DSM.GatewayEngine\IISLogGate")
        {
            XConsole.SetDefaultColorSet(ConsoleColorSetGreen.Instance);

            Get(WebOperations.WebMethod.GET_TRANSACTION_LASTLOGDATE_BY_SITEID.WithParamSiteId(), new Func<dynamic, object>(GetLastLogDate));
            Get(WebOperations.WebMethod.GET_TRANSACTION_ACTIVATION_STATUS, new Func<dynamic, object>(GetActivationStatus));
            Get(WebOperations.WebMethod.GET_TRANSACTIONS_BY_MACHINENAME.WithParamMachineName(), new Func<dynamic, object>(GetTransactions));
            Get(WebOperations.WebMethod.GET_TRANSACTION_FILTERS, new Func<dynamic, object>(GetTransactionFilter));
            Post(WebOperations.WebMethod.POST_TRANSACTION, new Func<dynamic, object>(PostTransactions));
            Post(WebOperations.WebMethod.POST_TRANSACTION_MULTIPLE, new Func<dynamic, object>(PostTransactionsMulti));
        }

        private object GetTransactions(dynamic ServerName)
        {
            string serverName = ServerName;
            string startDate = Convert.ToDateTime(Request.Query["StartDate"]);
            string endDate = Convert.ToDateTime(Request.Query["EndDate"]);

            logManager.Write("[GET] GetTransactions");
            return Response.AsJson("Get Transactions By Server Name");
        }

        private object PostTransactions(dynamic parameters)
        {
            SiteTransaction newTransaction = this.Bind<SiteTransaction>();
            logManager.Write("[POST] PostTransactions");
            int result = _siteTransactionRepository.PostSiteTransaction(newTransaction);
            return Response.AsJson(result);
        }
        private object PostTransactionsMulti(dynamic arg)
        {
            SiteTransaction[] newTransaction = this.Bind<SiteTransaction[]>();
            logManager.Write("[POST] PostTransactions");
            IEnumerable<SiteTransaction> result = _siteTransactionRepository.PostSiteTransaction(newTransaction);
            return Response.AsJson(result);
        }

        private object GetTransactionFilter(dynamic parameters)
        {
            logManager.Write("[GET] GetTransactionFilter");
            IEnumerable<SiteTransactionFilterExcludedItem> filterExcludedItems = _siteTransactionRepository.GetFilters();
            return Response.AsJson(filterExcludedItems);
        }

        private object GetActivationStatus(dynamic parameters)
        {
            return Response.AsJson(true);
        }

        private object GetLastLogDate(dynamic parameters)
        {
            int siteId = parameters.SiteId;
            DateTime lastLogDate = _siteTransactionRepository.GetLastLogDateBySiteId(siteId);
            return Response.AsJson(lastLogDate);
        }
    }
}
