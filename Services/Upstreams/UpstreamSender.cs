using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace iss_data.Services.Upstreams
{
    public abstract class UpstreamSender
    {
        
        protected readonly IssTelemetryStatistics Statistics;
        protected Queue<string> MessageQueue = new Queue<string>();
        private readonly ILogger _logger;
        private readonly Timer _batchTimer;

        protected UpstreamSender(ILogger logger, IssTelemetryStatistics statistics)
        {
            _logger = logger;
            Statistics = statistics;
            _batchTimer = new Timer(SendPartialBatch, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

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
                MessageQueue.Enqueue(message);
                _logger.LogTrace(message);
                if (MessageQueue.Count < 100) return;
                SendBatch();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        protected void UpdateLastSentTime() => _lastSendTime = DateTime.UtcNow;
        protected abstract Task SendBatch();
        public Task Flush() => SendBatch();
    }
}