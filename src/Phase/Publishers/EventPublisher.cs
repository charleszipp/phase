using Phase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Publishers
{
    internal class EventPublisher
    {
        private readonly DependencyResolver _resolver;

        public EventPublisher(DependencyResolver resolver) => _resolver = resolver;

        internal void Publish(IEnumerable<IEvent> events, CancellationToken cancellationToken)
        {
            foreach(var e in events.OrderBy(e => e.SequenceId))
            {
                if (!cancellationToken.IsCancellationRequested)
                    Publish(e, cancellationToken);
            }
        }

        internal virtual void Publish(IEvent @event, CancellationToken cancellationToken)
        {
            var subscribers = _resolver.GetEventSubscribers(@event);
            if(subscribers?.Any() ?? false)
            {
                foreach(var subscriber in subscribers)
                {
                    subscriber.Publish(@event);
                }
            }
        }
    }
}
