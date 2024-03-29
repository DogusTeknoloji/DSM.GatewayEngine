﻿using DSM.Core.Ops;
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
        internal static LogManager _logManager = LogManager.GetManager("DSM.GatewayEngine");

        private readonly IWebHost _server;

        private static readonly IConfiguration _config = AppSettingsManager.GetConfiguration();

        public Gateway()
        {
            int port = _config.GetValue<int>("Host:Port");
            string localIpAddr = Core.Ops.Extensions.GetLocalIPAddress();
            IList<string> hosts = new List<string>
            {
                $"http://localhost:{port}",
                $"http://{localIpAddr}:{port}"
            };

            _logManager.Write($"Hosts -> {string.Join(", ", hosts)}");

            _server = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls(hosts.ToArray())
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.AllowSynchronousIO = true;
                })
                .Build();

            _server.Run();

            XConsole.SetTitle("DSM Gateway");
            _logManager.Write($"Gateway RUNNING UP!");
        }

        ~Gateway()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_server != null)
                {
                    _server.StopAsync();
                    _server.Dispose();
                }
            }
        }
    }
}
