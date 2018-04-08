using System.Threading;
using System.Threading.Tasks;

namespace Phase.Commands
{
    public interface IHandleQuery<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> Execute(TQuery query, CancellationToken cancellationToken);
    }
}