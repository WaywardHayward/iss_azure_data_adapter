using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Logging;

namespace iss_data.Services
{

    public class EventHubSender
    {
        private readonly string _connectionString;
        private readonly string _eventHubName;
        private EventHubProducerClient _eventHubClient;
        private readonly ILogger<EventHubSender> logger;

        public EventHubSender(ILogger<EventHubSender> logger, IConfiguration configuration)
        {
            this.logger = logger;
            _connectionString = configuration["EVENT_HUB_CONNECTION_STRING"];
            _eventHubName = configuration["EVENT_HUB_NAME"];
            _eventHubClient = new EventHubProducerClient(_connectionString, _eventHubName);
        }

        private Queue<string> _messageQueue = new Queue<string>();

        public async Task SendMessageAsync(string message)
        {
            try
            {
                _messageQueue.Enqueue(message);
                logger.LogInformation(message);
                if (_messageQueue.Count < 100)
                    return;

                var messages = new List<EventData>();
                var batchId = Guid.NewGuid().ToString();

                logger.LogInformation("Sending Batch: " + batchId);

                for (int i = 0; i < 100; i++)
                {
                    messages.Add(new EventData(Encoding.UTF8.GetBytes(_messageQueue.Dequeue())));
                }

                _eventHubClient.SendAsync(messages).Wait();

                logger.LogInformation("Send Batch: " + batchId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }


    }


}