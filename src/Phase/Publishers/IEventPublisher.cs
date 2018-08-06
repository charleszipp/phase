using System.Collections.Generic;
using System.Threading;
using Phase.Interfaces;

namespace Phase.Publishers
{
    internal interface IEventPublisher
    {
        void Publish(IEnumerable<IEvent> events, CancellationToken cancellationToken);
        void Publish(IEvent @event, CancellationToken cancellationToken);
    }
}