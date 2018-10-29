// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Microsoft Azure Event Hubs Client for .NET
// For samples see: https://github.com/Azure/azure-event-hubs/tree/master/samples/DotNet
// For documenation see: https://docs.microsoft.com/azure/event-hubs/
using System;
using Microsoft.Azure.EventHubs;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Townhall
{
    internal class Configuration
    {
        // Event Hub-compatible endpoint
        // az iot hub show --query properties.eventHubEndpoints.events.endpoint --name {your IoT Hub name}
        public string EventHubsCompatibleEndpoint { get; set; }

        // Event Hub-compatible name
        // az iot hub show --query properties.eventHubEndpoints.events.path --name {your IoT Hub name}
        public string EventHubsCompatiblePath { get; set; }

        // az iot hub policy show --name iothubowner --query primaryKey --hub-name {your IoT Hub name}
        public string IoTHubSasKey { get; set; }
    }

    internal class ReadD2CMessagesService : IHostedService
    {
        private readonly static string iotHubSasKeyName = "iothubowner";
        private readonly Configuration configuration;
        private readonly ILogger logger;
        private readonly EventsProcessor processor;
        private EventHubClient eventHubClient;

        public ReadD2CMessagesService(IOptions<Configuration> configuration, ILogger<ReadD2CMessagesService> logger, EventsProcessor processor)
        {
            this.configuration = configuration.Value;
            this.logger = logger;
            this.processor = processor;
        }

        // Asynchronously create a PartitionReceiver for a partition and then start 
        // reading any messages sent from the simulated client.
        private async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            // Create the receiver using the default consumer group.
            // For the purposes of this sample, read only messages sent since 
            // the time the receiver is created. Typically, you don't want to skip any messages.
            var eventHubReceiver = eventHubClient.CreateReceiver("$Default", partition, EventPosition.FromEnqueuedTime(DateTime.Now));
            this.logger.LogInformation("Create receiver on partition: " + partition);
            while (true)
            {
                if (ct.IsCancellationRequested) break;
                this.logger.LogInformation("Listening for messages on: " + partition);
                // Check for EventData - this methods times out if there is nothing to retrieve.
                var events = await eventHubReceiver.ReceiveAsync(100);

                this.processor.Process(events, partition);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create an EventHubClient instance to connect to the
            // IoT Hub Event Hubs-compatible endpoint.
            var connectionString = new EventHubsConnectionStringBuilder(
                new Uri(this.configuration.EventHubsCompatibleEndpoint),
                this.configuration.EventHubsCompatiblePath,
                iotHubSasKeyName,
                this.configuration.IoTHubSasKey);
            
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString.ToString());

            // Create a PartitionReciever for each partition on the hub.
            var runtimeInfo = await eventHubClient.GetRuntimeInformationAsync();
            var d2cPartitions = runtimeInfo.PartitionIds;

            var tasks = new List<Task>();
            foreach (string partition in d2cPartitions)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cancellationToken));
            }

            // Wait for all the PartitionReceivers to finsih.
            Task.WaitAll(tasks.ToArray());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}