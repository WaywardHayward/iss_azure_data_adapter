using System;
using Azure.Messaging.EventHubs.Producer;
using iss_data.Services.Face;
using iss_data.Services.Upstreams;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iss_data.Utils
{
    public static class ServiceCollectionHelpers
    {
        public static IServiceCollection AddUpstreams(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            var upstreamMode = GetUpstreamMode(configuration);
            return (IsEventHub(upstreamMode)) ? AddEventHubUpstream(services, configuration) : AddIoTHubUpstream(services, configuration);
        }

        private static string GetUpstreamMode(IConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration[SettingsConstants.UPSTREAM_MODE]))
                return SettingsConstants.UPSTREAM_MODE_EVENT_HUB;
            return configuration[SettingsConstants.UPSTREAM_MODE]?.Replace("_", string.Empty).Replace("-", string.Empty);
        }

        public static bool IsEventHub(string upstreamMode)
        {
            return upstreamMode.Equals(SettingsConstants.UPSTREAM_MODE_EVENT_HUB, StringComparison.OrdinalIgnoreCase);
        }

        private static IServiceCollection AddIoTHubUpstream(IServiceCollection services, IConfiguration configuration)
        {
            return services.AddSingleton<DeviceClient>((s) => DeviceClient.CreateFromConnectionString(GetUpstreamConnectionString(configuration))).AddSingleton<IUpstreamSender, IoTHubSender>();
        }

        private static IServiceCollection AddEventHubUpstream(IServiceCollection services, IConfiguration configuration)
        {
            var eventHubConnectionString = GetUpstreamConnectionString(configuration);
            var eventHubName = configuration[SettingsConstants.EVENT_HUB_NAME];

            if (string.IsNullOrWhiteSpace(eventHubName))
                throw new Exception($"Setting '{SettingsConstants.EVENT_HUB_NAME}' for Event Hub name is not set");

            return services.AddSingleton<EventHubProducerClient>(new EventHubProducerClient(eventHubConnectionString, eventHubName)).AddSingleton<IUpstreamSender, EventHubSender>();
        }

        private static string GetUpstreamConnectionString(IConfiguration configuration)
        {

            var connectionString = string.Empty;

            if (!string.IsNullOrWhiteSpace(configuration[SettingsConstants.UPSTREAM_CONNECTION_STRING]))
                connectionString = configuration[SettingsConstants.UPSTREAM_CONNECTION_STRING];
            else if (!string.IsNullOrWhiteSpace(configuration[SettingsConstants.IOT_HUB_CONNECTION_STRING]))
                connectionString = configuration[SettingsConstants.IOT_HUB_CONNECTION_STRING];
            else if (!string.IsNullOrWhiteSpace(configuration[SettingsConstants.EVENT_HUB_CONNECTION_STRING]))
                connectionString = configuration[SettingsConstants.EVENT_HUB_CONNECTION_STRING];

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception($"Upstream connection string string is not set, please set one of the following: {SettingsConstants.UPSTREAM_CONNECTION_STRING}, {SettingsConstants.IOT_HUB_CONNECTION_STRING}, {SettingsConstants.EVENT_HUB_CONNECTION_STRING}");

            return connectionString;
        }
    }
}