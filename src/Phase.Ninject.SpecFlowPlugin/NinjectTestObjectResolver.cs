using BoDi;
using Ninject;
using System;
using TechTalk.SpecFlow.Infrastructure;

namespace Phase.Ninject.SpecFlowPlugin
{
    public class NinjectTestObjectResolver : ITestObjectResolver
    {
        public object ResolveBindingInstance(Type bindingType, IObjectContainer container)
        {
            var kernel = container.Resolve<IKernel>();
            return kernel.Get(bindingType);
        }
    }
}