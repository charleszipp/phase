using Phase.Domains;
using System.Linq;

namespace Phase.Builders
{
    public static class PhaseVolatileStateFluent
    {
        public static IPhaseBuilder WithAggregateRoot<TAggregate>(this IPhaseBuilder builder)
            where TAggregate : AggregateRoot, IVolatileState => 
            new PhaseVolatileStateDecorator<TAggregate>(builder);

        public static IPhaseBuilder WithReadModel<TReadModel>(this IPhaseBuilder builder)
            where TReadModel : class, IVolatileState => 
            new PhaseVolatileStateDecorator<TReadModel>(builder);
    }

    public class PhaseVolatileStateDecorator<T> : PhaseDecorator
        where T : class, IVolatileState
    {
        public PhaseVolatileStateDecorator(IPhaseBuilder builder)
            : base(builder) { }

        public override Phase Build()
        {
            var rvalue = _builder.Build();
            rvalue.DependencyResolver.RegisterVolatileState<T>();
            return rvalue;
        }
    }
}