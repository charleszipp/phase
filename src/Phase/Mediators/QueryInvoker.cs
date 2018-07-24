using Phase.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Mediators
{
    internal abstract class QueryInvoker<TResult>
    {
        internal abstract Task<TResult> InvokeAsync(IQuery<TResult> query, DependencyResolver resolver, CancellationToken cancellationToken);
    }

    internal class QueryInvoker<TQuery, TResult> : QueryInvoker<TResult>
        where TQuery : IQuery<TResult>
    {
        internal override Task<TResult> InvokeAsync(IQuery<TResult> query, DependencyResolver resolver, CancellationToken cancellationToken) => 
            resolver.GetQueryHandler<TQuery, TResult>().Execute((TQuery)query, cancellationToken);
    }
}