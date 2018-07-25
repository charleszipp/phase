using System;

namespace Phase.SpecFlowPlugin
{
    public interface IPhaseFinder
    {
        Func<Phase> GetCreateScenarioPhaseInstance();
    }
}