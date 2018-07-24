using Phase.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Domains
{
    public abstract class CommandHandler
    {
        internal Session _session { get; set; }

        public string TenantId { get; internal set; }

        protected Task<TAggregate> GetAggregateAsync<TAggregate>(string id, CancellationToken cancellationToken)
            where TAggregate : AggregateRoot => _session.GetOrAddAsync<TAggregate>(id, cancellationToken);

        protected async Task RaiseEventAsync<TAggregate>(string id, IEvent @event, CancellationToken cancellationToken)
                    where TAggregate : AggregateRoot => await _session.ApplyEventsAsync<TAggregate>(id, cancellationToken, @event);
    }

    public abstract class CommandHandler<TCommand> : CommandHandler, IHandleCommand<TCommand>
        where TCommand : ICommand
    {
        public abstract Task Execute(TCommand command, CancellationToken cancellationToken);
    }
}