using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Domains
{
    /// Responsible for controlling the number of threads that can execute against a single aggregate root instance at any given time.
    /// </summary>
    /// <typeparam name="TAggregate">Aggregate root type</typeparam>
    public class AggregateProxy<TAggregate> : DispatchProxy
        where TAggregate : AggregateRoot
    {
        private TAggregate _aggregate;
        private readonly SemaphoreSlim _pool = new SemaphoreSlim(1, 1);

        public static TAggregate Create(TAggregate aggregate)
        {
            object proxy = Create<TAggregate, AggregateProxy<TAggregate>>();
            ((AggregateProxy<TAggregate>)proxy)._aggregate = aggregate;
            return (TAggregate)proxy;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            try
            {
                _pool.Wait();
                var result = targetMethod.Invoke(_aggregate, args);
                if (result is Task)
                    ((Task)result).Wait();

                return result;
            }
            catch (Exception ex) when (ex is TargetInvocationException)
            {
                throw ex.InnerException ?? ex;
            }
            finally
            {
                _pool.Release();
            }
        }
    }
}