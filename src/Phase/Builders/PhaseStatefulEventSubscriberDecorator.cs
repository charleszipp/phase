using Phase.Domains;
using Phase.Interfaces;

namespace Phase.Builders
{
    public static class PhaseStatefulEventSubscriberFluentDecorator
    {
        public static IPhaseBuilder WithStatefulEventSubscriber<TEvent, TEventHandler>(this IPhaseBuilder builder)
            where TEvent : IEvent
            where TEventHandler : class, IHandleEvent<TEvent>, IVolatileState => 
            new PhaseStatefulEventSubscriberDecorator<TEvent, TEventHandler>(builder);
    }

    public class PhaseStatefulEventSubscriberDecorator<TEvent, TEventHandler> : PhaseDecorator
        where TEvent : IEvent
        where TEventHandler : class, IHandleEvent<TEvent>, IVolatileState
    {
        public PhaseStatefulEventSubscriberDecorator(IPhaseBuilder builder) 
            : base(builder) { }

        public override Phase Build()
        {
            var rvalue = _builder.Build();
            rvalue.DependencyResolver.RegisterStatefulEventHandler<TEvent, TEventHandler>();
            return rvalue;
        }
    }
}