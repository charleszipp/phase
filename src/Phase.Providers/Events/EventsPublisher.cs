using Phase.Commands;
using Microsoft.ApplicationInsights;
using Phase.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Phase.Providers
{
    public class EventsPublisher : IEventsPublisher
    {
        protected readonly HandlersFactory _factory;
        protected readonly TelemetryClient _telemetryClient;

        public EventsPublisher(HandlersFactory factory, TelemetryClient tc)
        {
            _factory = factory;
            _telemetryClient = tc;
        }

        protected void FormatEvent(ref StringBuilder sb, IEvent e, int order, bool error)
        {
            sb.AppendLine(string.Format("{0,1}|{1,5}|{2,20}|{3,30}|{4,-20}|{5,30}|{6,-5}|{7,26}|{8,30}", new string[] { error ? "*" : " ", order.ToString(), e.SequenceId, e.Id, e.Type, e.EntityId.ToString(), e.Version.ToString(), e.Timestamp.ToString(), e.AggregateId }));
        }

        public void Publish(IEnumerable<IEvent> events)
        {
            int i = 0;
            // we want these to be sent to the subscribers in the order they occurred
            // if they are subscribed to multiple events, this will ensure they get them in the order
            // that they were produced
            foreach (var e in events.OrderBy(e => e.SequenceId))
            {
                try
                {
                    var invoker = (EventInvoker)Activator.CreateInstance(typeof(EventInvoker<>).MakeGenericType(e.GetType()));
                    invoker.Invoke(e, _factory);
                }
                catch (Exception ex)
                {
                    StringBuilder eventsDiagnostics = new StringBuilder();

                    eventsDiagnostics.AppendLine("There was a problem publishing event - number " + i.ToString() + " of " + events.Count().ToString());
                    eventsDiagnostics.AppendLine("\nSequence List");
                    eventsDiagnostics.AppendLine(string.Format("{0,1}|{1,5}|{2,20}|{3,30}|{4,-20}|{5,30}|{6,-5}|{7,26}|{8,30}", new string[] { "*", "Order", "SequenceId", "Id", "Type", "EntityId", "Version", "Timestamp", "AggregateId" }));
                    int i2 = 0;
                    foreach (var e2 in events.OrderBy(e2 => e2.SequenceId))
                    {
                        FormatEvent(ref eventsDiagnostics, e2, i2, (i == i2));
                         i2++;
                    }

                    _telemetryClient.TrackTrace(eventsDiagnostics.ToString());
                    throw;
                }
                i++;
            }
        }

        public Task PublishAsync(IEvent @event)
        {
            var invoker = (EventInvoker)Activator.CreateInstance(typeof(EventInvoker<>).MakeGenericType(@event.GetType()));
            return invoker.InvokeAsync(@event, _factory);
        }
    }
}