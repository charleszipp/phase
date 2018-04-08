using Phase.Commands;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Providers
{
    public interface IEventsProvider
    {
        Task<IEnumerable<IEvent>> ActivateAsync(string serviceInstanceName, CancellationToken cancellationToken);

        Task<IEnumerable<IEvent>> GetAsync(string aggregateId, int fromVersion = -1);

        Task CommitAsync(IEnumerable<IEvent> events);

        Task AbortAsync();
    }
}