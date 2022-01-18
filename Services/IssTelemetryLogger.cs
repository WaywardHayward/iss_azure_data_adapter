using System.Text.Json;
using iss_data.Model;
using Microsoft.Extensions.Logging;

namespace iss_data.Services
{
    public class IssTelemetryLogger
    {
        private readonly ILogger<IssTelemetryService> _logger;

        public IssTelemetryLogger(ILogger<IssTelemetryService> logger)
        {
            _logger = logger;
        }

        public void LogTelemetryUpdate(IssTelemetryUpdate update)
        {
            _logger.LogInformation(JsonSerializer.Serialize<IssTelemetryUpdate>(update));
        }
    }
}