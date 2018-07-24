using Phase.Interfaces;

namespace Phase.Builders
{
    public static class PhaseQueryFluentDecorator
    {
        public static IPhaseBuilder WithQueryHandler<TQueryHandler, TQuery, TResult>(this IPhaseBuilder builder)
            where TQueryHandler : class, IHandleQuery<TQuery, TResult>
            where TQuery : IQuery<TResult> => new PhaseQueryDecorator<TQueryHandler, TQuery, TResult>(builder);
    }

    public class PhaseQueryDecorator<TQueryHandler, TQuery, TResult> : PhaseDecorator
        where TQueryHandler : class, IHandleQuery<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        public PhaseQueryDecorator(IPhaseBuilder builder)
            : base(builder) { }

        public override Phase Build()
        {
            var rvalue = _builder.Build();
            rvalue.DependencyResolver.RegisterQueryHandler<TQueryHandler, TQuery, TResult>();
            return rvalue;
        }
    }
}