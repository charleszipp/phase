using Phase.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.States
{
    public class VacantState : PhaseState
    {
        public override Task ExecuteAsync(Phase phase, ICommand command, CancellationToken cancellationToken) => 
            throw new Exception("Phase must be occupied before executing commands and queries");

        public override async Task<PhaseState> OccupyAsync(Phase phase, string tenantId, CancellationToken cancellationToken)
        {
            await phase.EventsProvider.OccupyAsync(tenantId, cancellationToken).ConfigureAwait(false);
            var events = await phase.EventsProvider.GetEventsAsync(cancellationToken).ConfigureAwait(false);
            phase.Publisher.Publish(events, cancellationToken);
            phase.State = PhaseStates.Occupied;
            phase.TenantId = tenantId;

            return new OccupiedState();
        }

        public override Task<TResult> QueryAsync<TResult>(Phase phase, IQuery<TResult> query, CancellationToken cancellationToken) =>
            throw new Exception("Phase must be occupied before executing commands and queries");

        public override Task<PhaseState> VacateAsync(Phase phase, CancellationToken cancellationToken) =>
            throw new Exception("Phase is already vacant");
    }
}