using Phase.Interfaces;
using System;

namespace Phase.Publishers
{
    public abstract class EventSubscriber
    {
        public EventSubscriber(Type eventType)
        {
            EventType = eventType;
        }

        public Type EventType { get; }

        public abstract void Publish(IEvent @event);
    }

    public class EventSubscriber<TEvent> : EventSubscriber
        where TEvent : IEvent
    {
        public EventSubscriber(IHandleEvent<TEvent> eventHandler)
            : base(typeof(TEvent))
        {
            EventHandler = eventHandler;
        }

        public IHandleEvent<TEvent> EventHandler { get; }

        public override void Publish(IEvent @event)
        {
            if (@event.GetType() == EventType)
            {
                EventHandler.Handle((TEvent)@event);
            }
            else
            {
                throw new ArgumentException("Provided event type does not match the subcription.");
            }
        }
    }
}
