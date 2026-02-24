using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;

namespace PagamentoApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    logging.AddEventLog(new EventLogSettings()
                    {
                        SourceName = "PagamentoApi",
                            LogName = "PagamentoApi",
                            Filter = (x, y) => y >= LogLevel.Warning
                    });
                }
                logging.AddConsole();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                /*
                webBuilder.ConfigureKestrel(o =>
                {
                    o.ConfigureHttpsDefaults(o =>
                    o.ClientCertificateMode =
                    ClientCertificateMode.AllowCertificate);
                });*/
            });
    }
}