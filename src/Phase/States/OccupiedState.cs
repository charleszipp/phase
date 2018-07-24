using Phase.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.States
{
    public class OccupiedState : PhaseState
    {
        public override async Task ExecuteAsync(Phase phase, ICommand command, CancellationToken cancellationToken)
        {
            try
            {
                await phase.Mediator.ExecuteAsync(phase, command, cancellationToken).ConfigureAwait(false);
                // before commit make sure we havent been asked to shut down
                cancellationToken.ThrowIfCancellationRequested();
                var events = phase.Session.Flush();
                await phase.EventsProvider.CommitAsync(events, cancellationToken).ConfigureAwait(false);
                phase.Publisher.Publish(events, cancellationToken);
            }
            catch
            {
                phase.Session.Flush();
                throw;
            }
        }

        public override Task<PhaseState> OccupyAsync(Phase phase, string tenantId, CancellationToken cancellationToken)
        {
            throw new Exception("Phase is already occupied");
        }

        public override Task<TResult> QueryAsync<TResult>(Phase phase, IQuery<TResult> query, CancellationToken cancellationToken) =>
            phase.Mediator.Query(phase, query, cancellationToken);

        public override async Task<PhaseState> VacateAsync(Phase phase, CancellationToken cancellationToken)
        {
            await phase.EventsProvider.VacateAsync(cancellationToken);
            phase.DependencyResolver.ReleaseVolatileStates();
            phase.State = PhaseStates.Vacant;
            phase.TenantId = null;
            return new VacantState();
        }
    }
}