using Phase.Interfaces;

namespace Phase.Builders
{
    public static class PhaseStatelessEventSubscriberFluentDecorator
    {
        public static IPhaseBuilder WithStatelessEventSubscriber<TEvent, TEventHandler>(this IPhaseBuilder builder)
            where TEvent : IEvent
            where TEventHandler : class, IHandleEvent<TEvent> =>
            new PhaseStatelessEventSubscriberDecorator<TEvent, TEventHandler>(builder);
    }

    public class PhaseStatelessEventSubscriberDecorator<TEvent, TEventHandler> : PhaseDecorator
        where TEvent : IEvent
        where TEventHandler : class, IHandleEvent<TEvent>
    {
        public PhaseStatelessEventSubscriberDecorator(IPhaseBuilder builder)
            : base(builder) { }

        public override Phase Build()
        {
            var rvalue = _builder.Build();
            rvalue.DependencyResolver.RegisterStatelessEventHandler<TEvent, TEventHandler>();
            return rvalue;
        }
    }
}