using Nancy;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DSM.GatewayEngine
{
    public class PublicGate : NancyModule
    {
        private static string VersionInfo
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                DateTime buildDate = new DateTime(2000, 1, 1)
                                        .AddDays(version.Build).AddSeconds(version.Revision * 2);
                string displayableVersion = $"{version} ({buildDate.ToLongDateString()})";
                return displayableVersion;
            }
        }

        public PublicGate()
        {
            Get("/", new Func<dynamic, object>(Index));
            Get("/howto", new Func<dynamic, object>(HowTo));
        }

        private object HowTo(dynamic arg)
        {
            List<ApiModel> apis = ViewHelper.GetApiGuides();
            XModel Model = new XModel
            {
                Apis = apis,
                Title = "Deneme Title'ı",
                Version = $"{VersionInfo}"
            };
            return View["api", model: Model];
        }

        private object Index(dynamic parameters)
        {
            //string path = $@"{AssemblyDirectory}\NGIndex\index.html";
            //StreamReader reader = new StreamReader(path);
            //string htmlContent = reader.ReadToEnd();
            //reader.Close(); reader.Dispose();
            //htmlContent = htmlContent.Replace("{PageLoadedDate}", DateTime.Now.ToString());
            //htmlContent = htmlContent.Replace("{Year}", DateTime.Now.Year.ToString());
            //htmlContent = htmlContent.Replace("{BulidNumber}", VersionInfo);
            //Gateway.Log($"Request GET->Index Page");
            //return View[viewName: null, model: htmlContent];
            XModel Model = new XModel
            {
                Title = "Deneme Title'ı",
                Version = $"{VersionInfo}",
                Bulid1 = "V1",
                Bulid2 = "V2",
                Bulid3 = "V3"
            };

            return View["master", model: Model];
        }
    }

    public class XModel
    {
        public string Title { get; set; }
        public string Version { get; set; }
        public string Bulid1 { get; set; }
        public string Bulid2 { get; set; }
        public string Bulid3 { get; set; }

        public List<ApiModel> Apis { get; set; }
    }
}
