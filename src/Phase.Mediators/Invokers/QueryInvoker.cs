using Phase.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Mediators
{
    public abstract class QueryInvoker<TResult>
    {
        public abstract Task<TResult> Invoke(IQuery<TResult> query, HandlerFactory factory, CancellationToken token);

        protected TQuery GetQueryHandler<TQuery>(HandlerFactory factory)
        {
            return (TQuery)factory(typeof(TQuery));
        }
    }

    public class QueryInvoker<TQuery, TResult> : QueryInvoker<TResult>
        where TQuery : IQuery<TResult>
    {
        public override Task<TResult> Invoke(IQuery<TResult> query, HandlerFactory factory, CancellationToken token)
        {
            return GetQueryHandler<IHandleQuery<TQuery, TResult>>(factory)
                .Execute((TQuery)query, token);
        }
    }
}