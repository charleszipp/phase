using System;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Infrastructure;

namespace Phase.SpecFlowPlugin
{
    public class PhaseTestObjectResolver : ITestObjectResolver
    {
        public object ResolveBindingInstance(Type bindingType, IObjectContainer container)
        {
            var phase = container.Resolve<Phase>();
            //todo: need to figure out a way to inject the phase object into the specflow binding class without a circular reference.
        }
    }
}