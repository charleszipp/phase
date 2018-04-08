using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Phase.Commands;

namespace Phase.Domains
{
    public class Session : ISession
    {
        private readonly IDictionary<string, AggregateDescriptor> _aggregates = new Dictionary<string, AggregateDescriptor>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly IRepository _repo;

        public Session(IRepository repo) => _repo = repo;

        public async Task<T> GetAsync<T>(Guid entityId)
            where T : AggregateRoot
        {
            T rvalue = null;

            var key = AggregateRoot.GetAggregateKeyFromEntityId<T>(entityId);

            if (_aggregates.ContainsKey(key))
            {
                _lock.EnterReadLock();
                try
                {
                    rvalue = (T)_aggregates[key].AggregateRoot;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            else
            {
                rvalue = await _repo.Get<T>(key).ConfigureAwait(false);
                await AddAsync(rvalue).ConfigureAwait(false);
            }

            return rvalue;
        }

        public Task AddAsync<T>(T aggregateRoot) 
            where T : AggregateRoot
        {
            _lock.EnterWriteLock();
            try
            {
                if (!_aggregates.ContainsKey(aggregateRoot.AggregateId))
                    _aggregates.Add(aggregateRoot.AggregateId, new AggregateDescriptor { AggregateRoot = aggregateRoot, Version = aggregateRoot.Version });

                return Task.Delay(0);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public IEnumerable<IEvent> Flush()
        {
            var rvalues = _aggregates.Values.SelectMany(a => a.AggregateRoot.Flush().ToList()).ToList();
            _aggregates.Clear();
            return rvalues;
        }
    }
}