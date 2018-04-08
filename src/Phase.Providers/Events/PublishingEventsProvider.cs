using Phase.Commands;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Providers
{
    public class PublishingEventsProvider : IEventsProvider
    {
        private readonly IEventsPublisher _eventsPublisher;
        private readonly IEventsProvider _eventsProvider;

        public PublishingEventsProvider(IEventsPublisher eventsPublisher, IEventsProvider eventsProvider)
        {
            _eventsPublisher = eventsPublisher;
            _eventsProvider = eventsProvider;
        }

        public Task AbortAsync()
        {
            return _eventsProvider.AbortAsync();
        }

        public async Task<IEnumerable<IEvent>> ActivateAsync(string serviceInstanceName, CancellationToken cancellationToken)
        {
            var rvalues = await _eventsProvider.ActivateAsync(serviceInstanceName, cancellationToken).ConfigureAwait(false);
            _eventsPublisher.Publish(rvalues);
            return rvalues;
        }

        public async Task CommitAsync(IEnumerable<IEvent> events)
        {
            await _eventsProvider.CommitAsync(events).ConfigureAwait(false);
            _eventsPublisher.Publish(events);
        }

        public Task<IEnumerable<IEvent>> GetAsync(string aggregateId, int fromVersion = -1)
        {
            return _eventsProvider.GetAsync(aggregateId, fromVersion);
        }
    }
}