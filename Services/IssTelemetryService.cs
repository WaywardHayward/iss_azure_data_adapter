using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using com.lightstreamer.client;
using iss_data.LightStreamer;
using iss_data.Model;
using iss_data.Services.Face;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace iss_data.Services
{
    public class IssTelemetryService : IHostedService
    {
        private readonly ILogger<IssTelemetryService> _logger;
        private readonly IssTelemetrySchema _issTelemetry;
        private readonly LightstreamerClient _issClient;
        private readonly IUpstreamSender _upstreamSender;
        private readonly IssTelemetryStatistics _statistics;
        private Subscription _issTelemetrySubscription;
        public EventHandler<IssTelemetryUpdate> OnUpdate;
        private readonly IConfiguration _configuration;

        public IssTelemetryService(ILogger<IssTelemetryService> logger, IssTelemetrySchema issTelemetrySchema, IUpstreamSender upstreamSender, IssTelemetryStatistics statistics, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _logger.LogInformation("Creating Iss Streaming Service");
            _issTelemetry = issTelemetrySchema;
            _issClient = new LightstreamerClient("https://push.lightstreamer.com/", "ISSLIVE");
            _upstreamSender = upstreamSender;
            _statistics = statistics;
        }

        public void Start()
        {
            if (_issTelemetrySubscription != null) return;
            InitializeSubscription();

            if (_upstreamSender != null)
                OnUpdate += (sender, update) => Task.Run(() => _upstreamSender.SendMessage(JsonSerializer.Serialize<IssTelemetryUpdate>(update)));

            _issClient.connect();
            _issClient.subscribe(_issTelemetrySubscription);
        }

        private void InitializeSubscription()
        {
            _logger.LogInformation("Initializing Iss Telemetry Subscription");
            _issTelemetrySubscription = new Subscription("MERGE", _issTelemetry.GetItems, new string[] { "TimeStamp", "Value", "Status.Class", "Status.Indicator", "Status.Color", "CalibratedData" });
            _issTelemetrySubscription.RequestedMaxFrequency = _configuration["MAX_UPDATE_FREQUENCY"] ?? "0.5";
            _logger.LogInformation("Requested Max Frequency: " + _issTelemetrySubscription.RequestedMaxFrequency);
            _issTelemetrySubscription.addListener(new IssTelemetryListener(HandleTelemetryUpdate, _logger, _statistics));
        }

        public void Stop()
        {
            if (_issTelemetrySubscription == null) return;
            _issClient.unsubscribe(_issTelemetrySubscription);
            _issClient.disconnect();
        }

        private void HandleTelemetryUpdate(ItemUpdate update)
        {
            var discipline = _issTelemetry.Disciplines.FirstOrDefault(s => s.Symbols.Any(s => s.PublicPUI == update.ItemName));
            var symbol = discipline.Symbols.FirstOrDefault(s => s.PublicPUI == update.ItemName);
            var telemetryUpdate = IssTelemetryUpdate.FromSymbol(symbol, update);
            telemetryUpdate.Discipline = discipline.Name;
            _statistics.IncrementMessagesReceived();
            OnUpdate?.Invoke(this, telemetryUpdate);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Iss Streaming Service");
            return Task.Run(() => Start());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Iss Streaming Service");
            return Task.Run(() => Stop());
        }
    }
}