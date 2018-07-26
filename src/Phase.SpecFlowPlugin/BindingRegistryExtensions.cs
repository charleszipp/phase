using TechTalk.SpecFlow.Bindings;
using System.Reflection;
using System.Collections.Generic;
using TechTalk.SpecFlow.Bindings.Reflection;
using System.Linq;

namespace Phase.SpecFlowPlugin
{
    public static class BindingRegistryExtensions
    {
        public static IEnumerable<IBindingType> GetBindingTypes(this IBindingRegistry bindingRegistry)
        {
            return bindingRegistry.GetStepDefinitions().Cast<IBinding>()
                .Concat(bindingRegistry.GetHooks().Cast<IBinding>())
                .Concat(bindingRegistry.GetStepTransformations())
                .Select(b => b.Method.Type)
                .Distinct();
        }

        public static IEnumerable<Assembly> GetBindingAssemblies(this IBindingRegistry bindingRegistry)
        {
            return bindingRegistry.GetBindingTypes().OfType<RuntimeBindingType>()
                .Select(t => t.Type.Assembly)
                .Distinct();
        }
    }
}