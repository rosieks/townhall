using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Townhall.Tests
{
    public class EventsProcessorTests
    {
        private readonly EventsProcessor target;

        public EventsProcessorTests(ITestOutputHelper output)
        {
            var logger = output.BuildLoggerFor<EventsProcessor>();  new Mock<ILogger<EventsProcessor>>();
            this.target = new EventsProcessor(logger);
        }

        [Fact]
        public void ShouldProcessMessages()
        {
            var eventData = new
            {
                messageId = 5,
            };
            var events = new[] { ToEventData(eventData) };

            this.target.Process(events, "partition");
        }

        private static EventData ToEventData<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return new EventData(Encoding.UTF8.GetBytes(json));
        }
    }
}
