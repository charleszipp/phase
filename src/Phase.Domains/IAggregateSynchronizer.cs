using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace Phase.Domains
{
    /// <summary>
    /// Interface for adding logic to be performed across multiple actor instances
    /// </summary>
    public interface IAggregateSynchronizer
    {
        object Invoke(MethodInfo method, object instance, object[] args);
    }
}