using Ninject;
using Phase.SpecFlowPlugin;
using System;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Bindings;

namespace Phase.Ninject.SpecFlowPlugin
{
    public class KernelFinder : IKernelFinder
    {
        private readonly IBindingRegistry _bindingRegistry;
        private readonly Lazy<Func<IKernel>> createScenarioKernel;

        public KernelFinder(IBindingRegistry bindingRegistry)
        {
            _bindingRegistry = bindingRegistry;
            createScenarioKernel = new Lazy<Func<IKernel>>(FindCreateScenarioKernel, true);
        }

        public Func<IKernel> GetScenarioKernelFactory()
        {
            var builder = createScenarioKernel.Value;
            if (builder == null)
                return () => new StandardKernel();
            return builder;
        }

        protected virtual Func<IKernel> FindCreateScenarioKernel()
        {
            var assemblies = _bindingRegistry.GetBindingAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var methodInfo in type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(m => Attribute.IsDefined((MemberInfo)m, typeof(ScenarioKernelFactoryAttribute))))
                    {
                        return () => (IKernel)methodInfo.Invoke(null, null);
                    }
                }
            }
            return null;
        }
    }
}