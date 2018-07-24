namespace Phase.Builders
{
    public abstract class PhaseDecorator : IPhaseBuilder
    {
        protected readonly IPhaseBuilder _builder;

        public PhaseDecorator(IPhaseBuilder builder)
        {
            _builder = builder;
        }

        public abstract Phase Build();
    }
}
