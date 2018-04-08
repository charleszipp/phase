using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Domains
{
    public class ThreadToggleConfig
    {
        public int MaxThreads { get; set; }
    }

    /// <summary>
    /// Responsible for controlling the number of different aggregates that can execute in parallel
    /// </summary>
    public class AggregateSynchronizer : IAggregateSynchronizer
    {
        private SemaphoreSlim _pool;
        private ThreadToggleConfig _config;
        
        public AggregateSynchronizer(ThreadToggleConfig config)
        {
            _config = config;
            _pool = new SemaphoreSlim(_config.MaxThreads, _config.MaxThreads);
        }

        public object Invoke(MethodInfo method, object instance, object[] args)
        {
            var pooltemp = _pool;
            try
            {
                pooltemp.Wait();

                var rvalue = method.Invoke(instance, args);
                if (rvalue is Task)
                    ((Task)rvalue).Wait();
                return rvalue;
            }
            finally
            {
                pooltemp.Release();
            }
        }
    }
}