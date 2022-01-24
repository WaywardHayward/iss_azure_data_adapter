using System;
using com.lightstreamer.client;
using Microsoft.Extensions.Logging;

namespace iss_data.LightStreamer
{
    public class IssTelemetryListener : SubscriptionListener
    {
        private readonly Action<ItemUpdate> _onUpdate;
        private readonly ILogger _logger;

        public IssTelemetryListener(Action<ItemUpdate> onUpdate, ILogger logger) {
            _onUpdate = onUpdate;
            _logger = logger;
        }

        void SubscriptionListener.onClearSnapshot(string itemName, int itemPos)
        {
            _logger.LogInformation("Clear Snapshot for " + itemName + ".");
        }

        void SubscriptionListener.onCommandSecondLevelItemLostUpdates(int lostUpdates, string key)
        {
            _logger.LogInformation("Lost Updates for " + key + " (" + lostUpdates + ").");
        }

        void SubscriptionListener.onCommandSecondLevelSubscriptionError(int code, string message, string key)
        {
            _logger.LogInformation("Subscription Error for " + key + ": " + message);
        }

        void SubscriptionListener.onEndOfSnapshot(string itemName, int itemPos)
        {
            _logger.LogInformation("End of Snapshot for " + itemName + ".");
        }

        void SubscriptionListener.onItemLostUpdates(string itemName, int itemPos, int lostUpdates)
        {
            _logger.LogInformation("Lost Updates for " + itemName + " (" + lostUpdates + ").");
        }

        void SubscriptionListener.onItemUpdate(ItemUpdate itemUpdate)
        {
            _onUpdate?.Invoke(itemUpdate);
        }

        void SubscriptionListener.onListenEnd(Subscription subscription)
        {
            _logger.LogInformation("Listen End for " + string.Join(",",subscription.Items) + ".");
        }

        void SubscriptionListener.onListenStart(Subscription subscription)
        {
            _logger.LogInformation("Listen Start for " + string.Join(",",subscription.Items) + ".");
        }

        void SubscriptionListener.onRealMaxFrequency(string frequency)
        {
             _logger.LogInformation("Real frequency: " + frequency + ".");
        }

        void SubscriptionListener.onSubscription()
        {
             _logger.LogInformation("Start subscription.");
        }

        void SubscriptionListener.onSubscriptionError(int code, string message)
        {
             _logger.LogError("Subscription error: " + message);
        }

        void SubscriptionListener.onUnsubscription()
        {
             _logger.LogInformation("Stop subscription.");
        }

    }
}
