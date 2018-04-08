using Phase.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Mediators
{
    public interface IMediator
    {
        Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken token);

        Task<T> ExecuteAsync<T>(ICommand<T> command, CancellationToken cancellationToken = default(CancellationToken));

        Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken));
    }
}