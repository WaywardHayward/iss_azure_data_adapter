using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Logging;
using System.Threading;
using iss_data.Services.Face;

namespace iss_data.Services.Upstreams
{

    public class EventHubSender : UpstreamSender, IUpstreamSender
    {
        private EventHubProducerClient _eventHubClient;
        private readonly ILogger<EventHubSender> _logger;
        public EventHubSender(ILogger<EventHubSender> logger, IConfiguration configuration, IssTelemetryStatistics statistics, EventHubProducerClient eventHubProducer)
        :base(logger, statistics)
        {
            _logger = logger;
            _eventHubClient = eventHubProducer;
        }
        protected override async Task SendBatch()
        {
            try
            {
                var messages = new List<EventData>();
                var batchId = Guid.NewGuid().ToString();

                _logger.LogInformation("Sending Batch: " + batchId);

                if (MessageQueue.Count == 0)
                {
                    _logger.LogInformation("No Messages for Batch: " + batchId);
                    return;
                }

                int messagesToDequeue = Math.Min(MessageQueue.Count, 100);

                lock (MessageQueue)
                {

                    for (int i = 0; i < messagesToDequeue; i++)
                    {
                        if (MessageQueue.Count == 0) break;
                        try
                        {
                            var message = MessageQueue.Dequeue();
                            if (message == null) continue;
                            messages.Add(new EventData(Encoding.UTF8.GetBytes(message)));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Failed to dequeue message", ex.Message);
                        }
                    }

                }

                await _eventHubClient.SendAsync(messages);

                Statistics.IncrementMessagesSent(messages.Count);
                _logger.LogInformation("Sent Batch: " + batchId);
                UpdateLastSentTime();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }


}