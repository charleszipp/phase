using System.Threading.Tasks;

namespace Phase.Commands
{
    /// <summary>
    /// Interface to subscribe to uncommitted events as they are raised.
    /// </summary>
    /// <typeparam name="TEvent">Type of event to subscribe to</typeparam>
    public interface ISubscribeVolatile<in TEvent>
        where TEvent : IEvent
    {
        Task WhenAsync(TEvent @event);
    }
}