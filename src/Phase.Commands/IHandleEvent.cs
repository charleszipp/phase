using System.Threading;
using System.Threading.Tasks;

namespace Phase.Commands
{
    public interface IHandleEvent<in TEvent>
        where TEvent : IEvent
    {
        void Handle(TEvent @event);
    }
}