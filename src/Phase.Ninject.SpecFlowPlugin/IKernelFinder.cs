using Ninject;
using System;

namespace Phase.Ninject.SpecFlowPlugin
{
    public interface IKernelFinder
    {
        Func<IKernel> GetScenarioKernelFactory();
    }
}