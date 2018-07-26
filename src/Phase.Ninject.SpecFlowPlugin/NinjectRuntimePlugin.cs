using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;

[assembly: RuntimePlugin(typeof(Phase.Ninject.SpecFlowPlugin.NinjectRuntimePlugin))]

namespace Phase.Ninject.SpecFlowPlugin
{
    public class NinjectRuntimePlugin : IRuntimePlugin
    {
        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters)
        {
            runtimePluginEvents.CustomizeGlobalDependencies += (sender, args) =>
            {
                if (!args.ObjectContainer.IsRegistered<IKernelFinder>())
                {
                    args.ObjectContainer.RegisterTypeAs<NinjectTestObjectResolver, ITestObjectResolver>();
                    args.ObjectContainer.RegisterTypeAs<KernelFinder, IKernelFinder>();

                    // workaround for parallel execution issue - this should be rather a feature in BoDi?
                    args.ObjectContainer.Resolve<IKernelFinder>();
                }
            };

            runtimePluginEvents.CustomizeScenarioDependencies += (sender, args) =>
            {
                args.ObjectContainer.RegisterFactoryAs(() =>
                {
                    var kernelFinder = args.ObjectContainer.Resolve<IKernelFinder>();
                    var phase = args.ObjectContainer.Resolve<Phase>();
                    var scenarioKernelFactory = kernelFinder.GetScenarioKernelFactory();
                    var kernel = scenarioKernelFactory();
                    kernel.Bind<Phase>().ToConstant(phase).InSingletonScope();
                    return kernel;
                });
            };
        }
    }
}