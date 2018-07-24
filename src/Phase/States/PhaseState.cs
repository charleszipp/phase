using Phase.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.States
{
    public abstract class PhaseState
    {
        public abstract Task<PhaseState> OccupyAsync(Phase phase, string tenantId, CancellationToken cancellationToken);
        public abstract Task<PhaseState> VacateAsync(Phase phase, CancellationToken cancellationToken);
        public abstract Task ExecuteAsync(Phase phase, ICommand command, CancellationToken cancellationToken);
        public abstract Task<TResult> QueryAsync<TResult>(Phase phase, IQuery<TResult> query, CancellationToken cancellationToken);
    }
}
