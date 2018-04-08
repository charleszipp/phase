using Phase.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Phase.Providers
{
    public interface IEventsPublisher
    {
        void Publish(IEnumerable<IEvent> events);

        Task PublishAsync(IEvent @event);
    }
}