using iss_data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iss_data.Controllers
{

    [ApiController]
    [Route("[controller]")]

    public class IssTelemetryStatisticsController : ControllerBase
    {
        private ILogger<IssTelemetryStatisticsController> _logger;
        private readonly IssTelemetryStatistics _statistics;

        public IssTelemetryStatisticsController(ILogger<IssTelemetryStatisticsController> logger, IssTelemetryStatistics statistics )
        {
            _logger = logger;
            _statistics = statistics;
        }

        [HttpGet]
        public IssTelemetryStatistics Get() => _statistics;


    }

}