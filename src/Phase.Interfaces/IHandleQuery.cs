using System.Threading;
using System.Threading.Tasks;

namespace Phase.Interfaces
{
    public interface IHandleQuery<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> Execute(TQuery query, CancellationToken cancellationToken);
    }
}