using Phase.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Mediators
{
    public class LockingMediator : IMediator
    {
        private readonly IMediator _mediator;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public LockingMediator(IMediator mediator) => _mediator = mediator;

        public async Task<T> ExecuteAsync<T>(ICommand<T> command, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await _lock.WaitAsync().ConfigureAwait(false);
                return await _mediator.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await _lock.WaitAsync().ConfigureAwait(false);
                await _mediator.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }

        public Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken token)
        {
            return _mediator.Query(query, token);
        }
    }
}