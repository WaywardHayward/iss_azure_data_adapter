using System.IO;
using System.Text.Json;
using iss_data.Model;
using iss_data.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddControllers();
            services.AddLogging();
            services.AddSingleton<IssTelemetrySchema>((s) => JsonSerializer.Deserialize<IssTelemetrySchema>(File.ReadAllText("Data/iss_telemetry_schema.json")));
            services.AddSingleton<EventHubSender>();
            services.AddHostedService<IssTelemetryService>();
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