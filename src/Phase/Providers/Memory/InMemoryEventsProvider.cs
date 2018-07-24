using Phase.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Providers.Memory
{
    public class InMemoryEventsProvider : IEventsProvider
    {
        protected readonly InMemoryEventCollection _db;
        protected readonly Func<string, IDictionary<string, string>> _tenantKeysFactory;
        protected string TenantInstanceName { get; private set; }

        public InMemoryEventsProvider(InMemoryEventCollection db, Func<string, IDictionary<string, string>> tenantKeysFactory)
        {
            _db = db;
            _tenantKeysFactory = tenantKeysFactory;
        }

        public virtual Task VacateAsync(CancellationToken cancellationToken)
        {
            TenantInstanceName = null;
            return Task.CompletedTask;
        }

        public virtual Task CommitAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken)
        {
            _db.AddOrUpdateEvents(events, _tenantKeysFactory(TenantInstanceName));
            return Task.CompletedTask;
        }

        public virtual Task<IEnumerable<IEvent>> GetAsync(string aggregateId, CancellationToken cancellationToken, int fromVersion = -1) =>
            Task.FromResult(_db.Get(aggregateId, fromVersion));

        public Task<IEnumerable<IEvent>> GetEventsAsync(CancellationToken cancellationToken) =>
            Task.FromResult(_db.Get(_tenantKeysFactory(TenantInstanceName)));

        public Task OccupyAsync(string tenantInstanceName, CancellationToken cancellationToken)
        {
            TenantInstanceName = tenantInstanceName;
            return Task.CompletedTask;
        }
    }
}