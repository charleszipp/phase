using Phase.Interfaces;

namespace Phase.Builders
{
    public static class PhaseCommandFluentDecorator
    {
        public static IPhaseBuilder WithCommandHandler<TCommandHandler, TCommand>(this IPhaseBuilder builder)
            where TCommandHandler : class, IHandleCommand<TCommand>
            where TCommand : ICommand => new PhaseCommandDecorator<TCommandHandler, TCommand>(builder);
    }

    public class PhaseCommandDecorator<TCommandHandler, TCommand> : PhaseDecorator
        where TCommandHandler : class, IHandleCommand<TCommand>
        where TCommand : ICommand
    {
        public PhaseCommandDecorator(IPhaseBuilder builder)
            : base(builder) { }

        public override Phase Build()
        {
            var rvalue = _builder.Build();
            rvalue.DependencyResolver.RegisterCommandHandler<TCommandHandler, TCommand>();
            return rvalue;
        }
    }
}