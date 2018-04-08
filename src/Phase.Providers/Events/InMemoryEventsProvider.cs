using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Phase.Commands;

namespace Phase.Providers
{
    public class InMemoryEventsProvider : IEventsProvider
    {
        protected readonly InMemoryEventCollection Db;
        protected readonly InMemoryEventsProviderConfiguration ProviderConfiguration;
        protected string ServiceInstanceName { get; private set; }
        public InMemoryEventsProvider(InMemoryEventCollection db, InMemoryEventsProviderConfiguration providerConfiguration)
        {
            Db = db;
            ProviderConfiguration = providerConfiguration;
        }
        public virtual Task AbortAsync()
        {
            ServiceInstanceName = null;
            return Task.CompletedTask;
        }
        public virtual Task<IEnumerable<IEvent>> ActivateAsync(string serviceInstanceName, CancellationToken cancellationToken)
        {
            ServiceInstanceName = serviceInstanceName;
            var serviceInstanceKeys = ProviderConfiguration.ServiceInstanceKeysFactory(serviceInstanceName);
            return Task.FromResult(Db.Get(serviceInstanceKeys));
        }
        public virtual Task CommitAsync(IEnumerable<IEvent> events)
        {
            var serviceInstanceKeys = ProviderConfiguration.ServiceInstanceKeysFactory(ServiceInstanceName);
            Db.AddOrUpdateEvents(events, serviceInstanceKeys);
            return Task.CompletedTask;
        }
        public virtual Task<IEnumerable<IEvent>> GetAsync(string aggregateId, int fromVersion = -1)
        {
            return Task.FromResult(Db.Get(aggregateId, fromVersion));
        }
    }
}