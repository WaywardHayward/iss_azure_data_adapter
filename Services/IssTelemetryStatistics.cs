using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace iss_data.Services
{
    public class IssTelemetryStatistics
    {
        private readonly ILogger<IssTelemetryStatistics> _logger;

        public int MessagesReceived { get; internal set; }

        public int MessagesSent {get; internal set;}

        public DateTime LastMessageReceived { get; internal set; }
        public DateTime LastMessageSent { get; internal set; }

        public string IssSubscriptionState {get; internal set;} = "Not Subscribed";

        private readonly Timer _outputTimer;

        public IssTelemetryStatistics(ILogger<IssTelemetryStatistics> logger)
        {
            _logger = logger;
            _outputTimer = new Timer(OutputStatistics, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(60));
        }

        private void OutputStatistics(object state)
        {
            _logger.LogInformation("Iss Subscription State : {0}", IssSubscriptionState);
            _logger.LogInformation("Messages Received: {0}", MessagesReceived);
            _logger.LogInformation("Messages Sent: {0}", MessagesSent);
            _logger.LogInformation("Last Message Received: {0}", LastMessageReceived);
            _logger.LogInformation("Last Message Sent: {0}", LastMessageSent);
        }

        public void Reset()
        {
            MessagesReceived = 0;
            MessagesSent = 0;
        }

        public void IncrementMessagesReceived()
        {
            LastMessageReceived = DateTime.UtcNow;
            MessagesReceived++;
        }

        public void IncrementMessagesSent(int by = 1)
        {
            LastMessageSent = DateTime.UtcNow;
            MessagesSent += by;
        }

        public void SetIssSubscriptionState(string state)
        {
            IssSubscriptionState = state;
        }

    }
}