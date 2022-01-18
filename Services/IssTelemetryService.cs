using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using com.lightstreamer.client;
using iss_data.LightStreamer;
using iss_data.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace iss_data.Services
{
    public class IssTelemetryService : IHostedService
    {
        private readonly ILogger<IssTelemetryService> _logger;
        private readonly IssTelemetrySchema _issTelemetry;
        private readonly LightstreamerClient _issClient;
        private readonly EventHubSender _eventHubSender;
        private Subscription _issTelemetrySubscription;
        public EventHandler<IssTelemetryUpdate> OnUpdate;

        public IssTelemetryService(ILogger<IssTelemetryService> logger, IssTelemetrySchema issTelemetrySchema, EventHubSender eventHubSender)
        {
            _logger = logger;
            _logger.LogInformation("Creating Iss Streaming Service");
            _issTelemetry = issTelemetrySchema;
            _issClient = new LightstreamerClient("https://push.lightstreamer.com/", "ISSLIVE");
            _eventHubSender = eventHubSender;
        }

        public void Start()
        {
            if (_issTelemetrySubscription != null) return;
            InitializeSubscription();

            if (_eventHubSender != null)
                OnUpdate += (sender, update) => _eventHubSender.SendMessageAsync(JsonSerializer.Serialize<IssTelemetryUpdate>(update));

            _issClient.connect();
            _issClient.subscribe(_issTelemetrySubscription);
        }

        private void InitializeSubscription()
        {
            _issTelemetrySubscription = new Subscription("MERGE", _issTelemetry.GetItems, new string[] { "TimeStamp", "Value", "Status.Class", "Status.Indicator", "Status.Color", "CalibratedData" });
            _issTelemetrySubscription.RequestedMaxFrequency = "3.0";
            _issTelemetrySubscription.addListener(new IssTelemetryListener(HandleTelemetryUpdate));
        }

        public void Stop()
        {
            if (_issTelemetrySubscription == null) return;
            _issClient.unsubscribe(_issTelemetrySubscription);
            _issClient.disconnect();
        }

        private void HandleTelemetryUpdate(ItemUpdate update)
        {
            var discipline =  _issTelemetry.Disciplines.FirstOrDefault(s => s.Symbols.Any(s => s.PublicPUI == update.ItemName));
            var symbol = discipline.Symbols.FirstOrDefault(s => s.PublicPUI == update.ItemName);
            var telemetryUpdate = IssTelemetryUpdate.FromSymbol(symbol, update);
            telemetryUpdate.Discipline = discipline.Name;
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