using System;
using TechTalk.SpecFlow.Bindings;
using System.Reflection;
using System.Linq;

namespace Phase.SpecFlowPlugin
{
    public class PhaseFinder : IPhaseFinder
    {
        private readonly IBindingRegistry _bindingRegistry;
        private readonly Lazy<Func<Phase>> _createScenarioPhaseInstance;

        public PhaseFinder(IBindingRegistry bindingRegistry)
        {
            _bindingRegistry = bindingRegistry;
            _createScenarioPhaseInstance = new Lazy<Func<Phase>>(FindCreateScenarioPhaseInstance, true);
        }

        public Func<Phase> GetCreateScenarioPhaseInstance()
        {
            var builder = _createScenarioPhaseInstance.Value;
            if (builder == null)
                throw new Exception("Unable to find scenario dependencies! Mark a static method that returns a Phase instance with [ScenarioDependencies]!");
            return builder;
        }

        protected virtual Func<Phase> FindCreateScenarioPhaseInstance()
        {
            var assemblies = _bindingRegistry.GetBindingAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var methodInfo in type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(m => Attribute.IsDefined((MemberInfo)m, typeof(ScenarioPhaseFactoryAttribute))))
                    {
                        return () => (Phase)methodInfo.Invoke(null, null);
                    }
                }
            }
            return null;
        }
    }
}