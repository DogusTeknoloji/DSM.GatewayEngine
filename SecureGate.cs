using DSM.Core.Models;
using DSM.Core.Models.LogServices;
using DSM.Core.Ops;
using DSM.Core.Ops.ConsoleTheming;
using DSM.OrmService;
using DSM.OrmService.AppServices;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DSM.GatewayEngine
{
    public partial class SecureGate : SecureModule
    {

        #region RepositoryDefinitions
        private readonly SiteRepository _siteRepository = new SiteRepository();
        private readonly SiteBindingRepository _siteBindingRepository = new SiteBindingRepository();
        private readonly SiteLogRepository _siteLogRepository = new SiteLogRepository();
        private readonly SiteWebConfigurationRepository _webConfigurationRepository = new SiteWebConfigurationRepository();
        private readonly SitePackageRepository _sitePackageRepository = new SitePackageRepository();

        private readonly MailQueueRepository _mailQueueRepository = new MailQueueRepository();
        private readonly MailRecipientsRepository _mailRecipientsRepository = new MailRecipientsRepository();

        #endregion

        public SecureGate() : base(@"DSM.GatewayEngine\SecureGate")
        {
            XConsole.SetDefaultColorSet(ConsoleColorSetGreen.Instance);

            Get(WebOperations.WebMethod.GET_SITES_ALL, new Func<dynamic, object>(GetSiteAll));
            Get(WebOperations.WebMethod.GET_MAILS_ALL, new Func<dynamic, object>(GetMails));
            Get(WebOperations.WebMethod.GET_MAIL_RECIPIENTS, new Func<dynamic, object>(GetMailRecipients));

            Get(WebOperations.WebMethod.GET_SITES_BY_MACHINENAME.WithParamMachineName(), new Func<dynamic, object>(GetSiteByMachineName));
            Get(WebOperations.WebMethod.GET_WEB_CONFIGURATION_BY_SITEID.WithParamSiteId(), new Func<dynamic, object>(GetWebConfiguration));
            Get(WebOperations.WebMethod.GET_MAIL_FROM_QUEUE_BY_SITEID.WithParamSiteId(), new Func<dynamic, object>(GetMailFromQueue));
            Get(WebOperations.WebMethod.GET_SITE_BY_ID.WithParamId(), new Func<dynamic, object>(GetSite));
            Get(WebOperations.WebMethod.GET_BINDING_BY_SITEID.WithParamSiteId(), new Func<dynamic, object>(GetBinding));
            Get(WebOperations.WebMethod.GET_EVENT_LOG_BY_SITEID.WithParamSiteId(), new Func<dynamic, object>(GetLog));

            Post(WebOperations.WebMethod.POST_SITE, new Func<dynamic, object>(PostSite));
            Post(WebOperations.WebMethod.POST_BINDING, new Func<dynamic, object>(PostBinding));
            Post(WebOperations.WebMethod.POST_EVENT_LOG, new Func<dynamic, object>(PostLog));
            Post(WebOperations.WebMethod.POST_PACKAGE, new Func<dynamic, object>(PostPackageVersion));
            Post(WebOperations.WebMethod.POST_WEB_CONFIGURATION, new Func<dynamic, object>(PostWebConfiguration));

            Post(WebOperations.WebMethod.POST_BINDING_MULTIPLE, new Func<dynamic, object>(PostBindingMulti));
            Post(WebOperations.WebMethod.POST_SITE_MULTIPLE, new Func<dynamic, object>(PostSiteMulti));
            Post(WebOperations.WebMethod.POST_EVENT_LOG_MULTIPLE, new Func<dynamic, object>(PostEvenLogMulti));
            Post(WebOperations.WebMethod.POST_WEB_CONFIGURATION_MULTIPLE, new Func<dynamic, object>(PostWebConfigurationMulti));
            Post(WebOperations.WebMethod.POST_PACKAGE_MULTIPLE, new Func<dynamic, object>(PostPackageVersionMulti));

            Post(WebOperations.WebMethod.POST_MAIL_TO_QUEUE, new Func<dynamic, object>(PostMailToQueue));
            Post(WebOperations.WebMethod.POST_DELETE_MAIL, new Func<dynamic, object>(DeleteMail));

            Get(WebOperations.WebMethod.GET_CONFIG_BACKUP_STATUS, new Func<dynamic, object>(GetConfigBackupStatus));
            Post(WebOperations.WebMethod.POST_CONFIG_TO_BACKUP, new Func<dynamic, object>(PostConfigToBackup));
        }

        #region SiteOps
        private object GetSite(dynamic parameters)
        {
            Site site = this._siteRepository.GetSite(parameters.Id);
            logManager.Write($"[GET]-> GetSite Id: {site.Id}");
            return Response.AsJson(site);
        }
        private object GetSiteByMachineName(dynamic parameters)
        {
            List<Site> site = this._siteRepository.GetSites(parameters.MachineName);
            logManager.Write($"[GET]-> GetSiteByMachineName Count: {site.Count()}");
            return Response.AsJson(site);
        }
        private object GetSiteAll(dynamic parameters)
        {
            IEnumerable<Site> sites = _siteRepository.All();
            logManager.Write($"[GET]-> GetSiteAll Count: {sites.Count()}");
            return Response.AsJson(sites);
        }

        private object PostSite(dynamic parameters)
        {
            Site newSite = this.Bind<Site>();
            int result = _siteRepository.PostSite(newSite);
            logManager.Write($"[POST]-> PostSite Id: {result}");

            return Response.AsJson(result);
        }
        private object PostSiteMulti(dynamic arg)
        {
            Site[] newSite = this.Bind<Site[]>();
            IEnumerable<Site> result = _siteRepository.PostSite(newSite);
            logManager.Write($"[POST]-> PostSite Id: {result}");
            if (result == null)
            {
                return Response.AsJson(0b10);
            }
            return Response.AsJson(result);
        }
        #endregion

        #region SiteBindingOps
        private object GetBinding(dynamic parameters)
        {
            SiteBinding siteBinding = this._siteBindingRepository.GetSiteBinding(parameters.SiteId);
            logManager.Write($"[GET]-> GetBinding Id: {siteBinding.Id}");
            return Response.AsJson(siteBinding);
        }
        private object PostBinding(dynamic parameters)
        {
            SiteBinding newBinding = this.Bind<SiteBinding>();
            int result = _siteBindingRepository.PostSiteBinding(newBinding);
            logManager.Write($"[POST]-> PostBinding Id: {result}");
            return Response.AsJson(result);
        }
        private object PostBindingMulti(dynamic arg)
        {
            SiteBinding[] newBinding = this.Bind<SiteBinding[]>();

            IEnumerable<SiteBinding> result = _siteBindingRepository.PostSiteBinding(newBinding);
            logManager.Write($"[POST]-> PostBinding  {result}");

            return Response.AsJson(result);
        }
        #endregion

        #region SiteLogOps
        private object GetLog(dynamic parameters)
        {
            SiteLog siteLog = this._siteLogRepository.GetSiteLog(parameters.SiteId);
            logManager.Write($"[GET]-> GetLog Id: {siteLog.SiteId}");
            return Response.AsJson(siteLog);
        }
        private object PostLog(dynamic parameters)
        {
            SiteLog newLog = this.Bind<SiteLog>();
            int result = _siteLogRepository.PostSiteLog(newLog);
            logManager.Write($"[POST]-> PostLog Id: {result}");
            return Response.AsJson(result);
        }
        private object PostEvenLogMulti(dynamic arg)
        {
            SiteLog[] newLog = this.Bind<SiteLog[]>();
            IEnumerable<SiteLog> result = _siteLogRepository.PostSiteLog(newLog);
            logManager.Write($"[POST]-> PostLog Id: {result}");
            return Response.AsJson(result);
        }
        #endregion

        #region SiteWebConfigurationOps
        private object GetWebConfiguration(dynamic parameters)
        {
            logManager.Write($"[GET]-> GetWebConfiguration  Id: {parameters.SiteId}");
            SiteWebConfiguration webConfiguration = this._webConfigurationRepository.GetSiteWebConfiguration(parameters.SiteId);
            return Response.AsJson(webConfiguration);
        }
        private object PostWebConfiguration(dynamic parameters)
        {
            SiteWebConfiguration newWebConfiguration = this.Bind<SiteWebConfiguration>();
            int result = _webConfigurationRepository.PostSiteWebConfiguration(newWebConfiguration);
            logManager.Write($"[POST]-> PostWebConfiguration  Id: {result}");
            return Response.AsJson(result);
        }
        private object PostWebConfigurationMulti(dynamic arg)
        {
            SiteWebConfiguration[] newWebConfiguration = this.Bind<SiteWebConfiguration[]>();
            IEnumerable<SiteWebConfiguration> result = _webConfigurationRepository.PostSiteWebConfiguration(newWebConfiguration);
            logManager.Write($"[POST]-> PostWebConfiguration  Id: {result}");
            return Response.AsJson(result);
        }
        #endregion

        #region SiteAlertMailsOps
        private object GetMails(dynamic parameters)
        {
            IEnumerable<IISMailQueue> mails = _mailQueueRepository.All();
            logManager.Write($"[GET]-> GetMails Count: {mails.Count()}");
            return Response.AsJson(mails);
        }
        private object GetMailFromQueue(dynamic parameters)
        {
            IISMailQueue mail = this._mailQueueRepository.GetMail(parameters.SiteId);
            logManager.Write($"[GET]-> MailFromQueue Id:{mail.Id}");
            return Response.AsJson(mail);
        }
        private object PostMailToQueue(dynamic parameters)
        {
            IISMailQueue mail = this.Bind<IISMailQueue>();
            int result = _mailQueueRepository.PostMailToQueue(mail);
            logManager.Write($"[POST]-> PostMailToQueue Id:{result}");
            return Response.AsJson(result);
        }
        private object DeleteMail(dynamic parameters)
        {
            IISMailQueue mail = this.Bind<IISMailQueue>();
            int result = _mailQueueRepository.DeleteMail(mail);
            logManager.Write($"[POST]-> DeleteMail Id:{result}");
            return Response.AsJson(result);
        }
        #endregion

        #region SiteAlertMailRecipientsOps
        private object GetMailRecipients(dynamic parameters)
        {
            IEnumerable<IISAlertMailRecipient> recipients = _mailRecipientsRepository.All();
            logManager.Write($"[GET]-> GetMailRecipients Count: {recipients.Count()}");
            return Response.AsJson(recipients);
        }
        #endregion

        #region BackupWebConfigOps
        private object GetConfigBackupStatus(dynamic parameters)
        {
            return Response.AsJson(false);
        }
        private object PostConfigToBackup(dynamic parameters)
        {
            SiteConfigFile webConfiguration = this.Bind<SiteConfigFile>();
            string webConfigRootPath = FileOperations.AssemblyDirectory;
            string filePath = string.Empty;
            string fileDate = string.Format("{0:dd-MM-yyyy}", DateTime.Now.ToShortDateString());
            filePath = Path.Combine(webConfigRootPath, webConfiguration.MachineName, webConfiguration.SiteName, fileDate);
            bool isValidPath = filePath.AutoPathRepair();
            if (isValidPath)
            {
                StreamWriter fileWriter = new StreamWriter(filePath, append: false);
                fileWriter.Write(webConfiguration.ContentRaw);
                fileWriter.Flush();
                fileWriter.Close();
                fileWriter.Dispose();
            }
            string resultSet = $"IsValidPath:{isValidPath}, Path:{filePath}";
            return Response.AsJson(resultSet);
        }
        #endregion

        #region SitePackageInfo
        private object PostPackageVersion(dynamic parameters)
        {
            SitePackage newSite = this.Bind<SitePackage>();
            int result = _sitePackageRepository.PostPackageInformation(newSite);
            logManager.Write($"[POST]-> PostSite Id: {result}");
            return Response.AsJson(result);

        }
        private object PostPackageVersionMulti(dynamic arg)
        {
            SitePackage[] newSite = this.Bind<SitePackage[]>();
            IEnumerable<SitePackage> result = _sitePackageRepository.PostPackageInformation(newSite);
            logManager.Write($"[POST]-> PostSite Id: {result}");
            return Response.AsJson(result);
        }

        #endregion
    }
}
