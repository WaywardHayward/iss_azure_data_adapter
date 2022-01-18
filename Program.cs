using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using com.lightstreamer.client;
using iss_data.Model;
using iss_data.LightStreamer;
using iss_data.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace iss_data
{
    class Program
    {

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
            {
                if (context.HostingEnvironment.IsDevelopment())
                {
                    builder
                        .AddJsonFile("appsettings.Development.json") // general dev defaults go here
                        .AddJsonFile("appsettings.local.json", true); // sensitive items go here (excluded from git)
                }
                else
                {
                    builder.AddEnvironmentVariables();
                }
            })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
