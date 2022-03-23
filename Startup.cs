using System.IO;
using System.Text.Json;
using iss_data.Model;
using iss_data.Services;
using iss_data.Services.Face;
using iss_data.Services.Upstreams;
using iss_data.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace iss_data
{
   public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddSingleton<IssTelemetryStatistics>();
            services.AddSingleton<IssTelemetrySchema>((s) => JsonSerializer.Deserialize<IssTelemetrySchema>(File.ReadAllText("iss_telemetry_schema.json")));
            services.AddUpstreams();
            services.AddHostedService<IssTelemetryService>();
            services.AddControllers();
        }

       

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}