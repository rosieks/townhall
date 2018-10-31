using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Townhall
{
    public class EventsProcessor
    {
        private ILogger logger;

        public EventsProcessor(ILogger<EventsProcessor> logger)
        {
            this.logger = logger;
        }

        public void Process(IEnumerable<EventData> events, string partition)
        {
            // If there is data in the batch, process it.
            if (events == null) return;

            foreach (EventData eventData in events)
            {
                string data = Encoding.UTF8.GetString(eventData.Body.Array);
                this.logger.LogInformation("Message received on partition {0}:", partition);
                this.logger.LogInformation("  {0}:", data);
                this.logger.LogInformation("Application properties (set by device):");
                foreach (var prop in eventData.Properties)
                {
                    this.logger.LogInformation("  {0}: {1}", prop.Key, prop.Value);
                }

                if (eventData.SystemProperties != null)
                {
                    this.logger.LogInformation("System properties (set by IoT Hub):");
                    foreach (var prop in eventData.SystemProperties)
                    {
                        this.logger.LogInformation("  {0}: {1}", prop.Key, prop.Value);
                    }
                }
            }
        }
    }
}
