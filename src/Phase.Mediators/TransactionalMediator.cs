using Phase.Commands;
using Phase.Domains;
using Phase.Providers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Mediators
{
    public class TransactionalMediator : IMediator
    {
        private readonly IEventsProvider _eventsProvider;
        private readonly IMediator _mediator;
        private readonly ISession _session;

        public TransactionalMediator(IMediator mediator, IEventsProvider eventsProvider, ISession session)
        {
            _mediator = mediator;
            _eventsProvider = eventsProvider;
            _session = session;
        }

        public async Task<T> ExecuteAsync<T>(ICommand<T> command,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new CommandCancelledException("A command waiting to execute encountered an abort.");
                var rvalue = await _mediator.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
                var events = _session.Flush();
                await _eventsProvider.CommitAsync(events).ConfigureAwait(false);
                return rvalue;
            }
            catch (Exception)
            {
                _session.Flush();
                throw;
            }
        }

        public async Task ExecuteAsync(ICommand command,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await _mediator.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
                var events = _session.Flush();
                await _eventsProvider.CommitAsync(events).ConfigureAwait(false);
            }
            catch (Exception)
            {
                _session.Flush();
                throw;
            }
        }

        public Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken token)
        {
            return _mediator.Query(query, token);
        }
    }
}