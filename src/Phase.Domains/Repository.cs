using Phase.Providers;
using System.Threading.Tasks;

namespace Phase.Domains
{
    public class Repository : IRepository
    {
        private readonly IEventsProvider _eventsProvider;
        private readonly IEventsPublisher _publisher;
        private readonly IAggregateSynchronizer _sync;
        private readonly AggregateFactory _resolver;

        public Repository(IEventsProvider eventsProvider, IEventsPublisher publisher, IAggregateSynchronizer sync, AggregateFactory resolver)
        {
            _eventsProvider = eventsProvider;
            _sync = sync;
            _resolver = resolver;
            _publisher = publisher;
        }

        public async Task<T> Get<T>(string aggregateId)
            where T : AggregateRoot
        {
            var events = await _eventsProvider.GetAsync(aggregateId).ConfigureAwait(false);
            var rvalue = AggregateProxy<T>.Create((T)_resolver(typeof(T)), _sync);
            rvalue.Load(_publisher, events, aggregateId);
            return rvalue;
        }
    }
}