using Phase.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phase.Providers
{
    public delegate IEnumerable<object> HandlersFactory(Type eventHandlerType);

    public abstract class EventInvoker
    {
        public abstract void Invoke(IEvent @event, HandlersFactory factory);

        public abstract Task InvokeAsync(IEvent @event, HandlersFactory factory);

        protected IEnumerable<TEventHandler> GetEventHandlers<TEventHandler>(HandlersFactory factory)
        {
            return factory(typeof(TEventHandler)).Select(x => (TEventHandler)x);
        }
    }

    public class EventInvoker<TEvent> : EventInvoker
        where TEvent : IEvent
    {
        public override void Invoke(IEvent @event, HandlersFactory factory)
        {
            var handlers = GetEventHandlers<IHandleEvent<TEvent>>(factory);
            if(handlers?.Any() ?? false)
                Parallel.ForEach(handlers, (handler) => handler.Handle((TEvent)@event));
        }

        public override async Task InvokeAsync(IEvent @event, HandlersFactory factory)
        {
            if (@event.IsVolatile)
            {
                var subscribers = GetEventHandlers<ISubscribeVolatile<TEvent>>(factory);
                if(subscribers?.Any() ?? false)
                    await Task.WhenAll(subscribers.Select(s => s.WhenAsync((TEvent)@event))).ConfigureAwait(false);
            }
            else
                Invoke(@event, factory);
        }
    }
}