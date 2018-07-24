using System.Threading;
using System.Threading.Tasks;

namespace Phase.Interfaces
{
    public interface IHandleEvent<in TEvent>
        where TEvent : IEvent
    {
        void Handle(TEvent @event);
    }

    public interface IHandleEventAsync<in TEvent>
        where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }
}