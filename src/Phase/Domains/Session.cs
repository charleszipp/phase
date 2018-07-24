using Phase.Interfaces;
using Phase.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Domains
{
    internal sealed class Session
    {
        private readonly IDictionary<string, AggregateDescriptor> _aggregates = new Dictionary<string, AggregateDescriptor>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly DependencyResolver _resolver;
        private readonly IEventsProvider _events;

        public Session(DependencyResolver resolver, IEventsProvider events)
        {
            _resolver = resolver;
            _events = events;
        }

        internal IEnumerable<IEvent> Flush()
        {
            var rvalues = _aggregates.Values.SelectMany(a => a.Flush().ToList()).ToList();
            _aggregates.Clear();
            return rvalues;
        }        

        internal async Task<T> GetOrAddAsync<T>(string entityId, CancellationToken cancellationToken)
            where T : AggregateRoot => (T)(await GetOrAddDescriptorAsync<T>(entityId, cancellationToken)).AggregateRoot;

        internal async Task ApplyEventsAsync<T>(string entityId, CancellationToken cancellationToken, params IEvent[] events)
            where T : AggregateRoot => (await GetOrAddDescriptorAsync<T>(entityId, cancellationToken)).Apply(events);

        private async Task<AggregateDescriptor> GetOrAddDescriptorAsync<T>(string entityId, CancellationToken cancellationToken)
            where T : AggregateRoot
        {
            var aggregateId = GetAggregateIdFromEntityId<T>(entityId);

            if (!_aggregates.ContainsKey(aggregateId))
            {
                await CreateAsync<T>(aggregateId, cancellationToken);
            }

            _lock.EnterReadLock();
            try
            {
                return _aggregates[aggregateId];
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        private async Task<T> CreateAsync<T>(string aggregateId, CancellationToken cancellationToken)
            where T : AggregateRoot
        {
            var root = _resolver.GetAggregateRoot<T>();
            var descriptor = new AggregateDescriptor(aggregateId, root);
            var eventHistory = await _events.GetAsync(aggregateId, cancellationToken).ConfigureAwait(false);
            descriptor.Replay(eventHistory);

            _lock.EnterWriteLock();
            try
            {
                if (!_aggregates.ContainsKey(aggregateId))
                    _aggregates.Add(aggregateId, descriptor);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            return root;
        }

        private string GetAggregateIdFromEntityId<T>(string entityId)
        {
            var aggregateType = typeof(T);
            var aggAttribute = aggregateType.GetCustomAttribute<PhaseAggregateAttribute>(false);
            if (aggAttribute == null)
                throw new Exception($"Aggregate type of {aggregateType.FullName} does not define an aggregate name. Use AggregateAttribute to define the aggregate name.");
            var uri = "/" + string.Join("/", aggAttribute.AggregateName, entityId);
            return new Uri(uri, UriKind.Relative).ToString();
        }
    }
}