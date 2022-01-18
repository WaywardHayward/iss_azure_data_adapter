using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace iss_data.Services {

    public class EventHubSender
    {
        private readonly string _connectionString;
        private readonly string _eventHubName;
        private EventHubProducerClient _eventHubClient;
        
        public EventHubSender (IConfiguration configuration)
        {
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

                if(_messageQueue.Count < 100)
                    return;

                var messages = new List<EventData>();
                var batchId = Guid.NewGuid().ToString();

                Console.WriteLine("Sending Batch: " + batchId);

                for (int i = 0; i < 100; i++)
                {
                    messages.Add(new EventData(Encoding.UTF8.GetBytes(_messageQueue.Dequeue())));
                }

                _eventHubClient.SendAsync(messages).Wait();

                Console.WriteLine("Send Batch: " + batchId);
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
        }


    }


}