using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace iss_data.Services
{

    public class EventHubSender
    {
        private readonly string _connectionString;
        private readonly string _eventHubName;
        private EventHubProducerClient _eventHubClient;
        private readonly IssTelemetryStatistics _statistics;
        private readonly Timer _batchTimer;
        private readonly ILogger<EventHubSender> _logger;

        public EventHubSender(ILogger<EventHubSender> logger, IConfiguration configuration, IssTelemetryStatistics statistics)
        {
            _logger = logger;
            _connectionString = configuration["EVENT_HUB_CONNECTION_STRING"];
            _eventHubName = configuration["EVENT_HUB_NAME"];
            _eventHubClient = new EventHubProducerClient(_connectionString, _eventHubName);
            _statistics = statistics;
            _batchTimer = new Timer(SendPartialBatch, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        private Queue<string> _messageQueue = new Queue<string>();
        private DateTime _lastSendTime;

        private void SendPartialBatch(object state)
        {
            if (_lastSendTime > DateTime.UtcNow.AddSeconds(-30)) return;
            SendBatch();
        }

        public void SendMessage(string message)
        {
            try
            {
                _messageQueue.Enqueue(message);
                _logger.LogTrace(message);
                if (_messageQueue.Count < 100) return;
                SendBatch();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private void SendBatch()
        {
            try
            {
                var messages = new List<EventData>();
                var batchId = Guid.NewGuid().ToString();

                _logger.LogInformation("Sending Batch: " + batchId);

                if (_messageQueue.Count == 0)
                {
                    _logger.LogInformation("No Messages for Batch: " + batchId);
                    return;
                }

                int messagesToDequeue = Math.Min(_messageQueue.Count, 100);

                lock (_messageQueue)
                {

                    for (int i = 0; i < messagesToDequeue; i++)
                    {
                        if(_messageQueue.Count == 0) break;
                        messages.Add(new EventData(Encoding.UTF8.GetBytes(_messageQueue.Dequeue())));
                    }

                }

                _eventHubClient.SendAsync(messages).Wait();

                _statistics.IncrementMessagesSent(messages.Count);
                _logger.LogInformation("Sent Batch: " + batchId);
                _lastSendTime = DateTime.UtcNow;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }


}