using DSM.Core.Ops;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DSM.GatewayEngine
{
    public class Gateway : IDisposable
    {
        internal static LogManager logManager = LogManager.GetManager("DSM.GatewayEngine");

        private readonly IWebHost _server;

        private static readonly IConfiguration config = AppSettingsManager.GetConfiguration();

        public Gateway()
        {
            int port = config.GetValue<int>("Host:Port");
            string localIpAddr = Core.Ops.Extensions.GetLocalIPAddress();
            IList<string> hosts = new List<string>
            {
                $"http://localhost:{port}",
                $"http://{localIpAddr}:{port}"
            };

            logManager.Write($"Hosts -> {string.Join(", ", hosts)}");

            _server = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls(hosts.ToArray())
                .UseStartup<Startup>()
                .UseKestrel()
                .Build();

            _server.Run();

            XConsole.SetTitle("DTIISM - Nancy Gateway");
            logManager.Write($"Gateway RUNNING UP!");
        }

        ~Gateway()
        {
            Dispose();
        }

        public void Dispose()
        {
            _server.StopAsync();
            _server.Dispose();
        }
    }
}
