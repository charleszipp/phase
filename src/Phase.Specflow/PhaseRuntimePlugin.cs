using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;

[assembly: RuntimePlugin(typeof(Phase.SpecFlowPlugin.PhaseRuntimePlugin))]

namespace Phase.SpecFlowPlugin
{
    public class PhaseRuntimePlugin : IRuntimePlugin
    {
        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters)
        {
            runtimePluginEvents.CustomizeGlobalDependencies += (sender, args) =>
            {
                if (!args.ObjectContainer.IsRegistered<IPhaseFinder>())
                {
                    args.ObjectContainer.RegisterTypeAs<PhaseFinder, IPhaseFinder>();

                    // workaround for parallel execution issue - this should be rather a feature in BoDi?
                    args.ObjectContainer.Resolve<IPhaseFinder>();
                }
            };

            runtimePluginEvents.CustomizeScenarioDependencies += (sender, args) =>
            {
                args.ObjectContainer.RegisterFactoryAs(() =>
                {
                    var phaseFinder = args.ObjectContainer.Resolve<IPhaseFinder>();
                    var createScenarioPhaseInstance = phaseFinder.GetCreateScenarioPhaseInstance();
                    return createScenarioPhaseInstance();
                });
            };
        }
    }
}