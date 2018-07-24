using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Phase.Ninject
{
    public class NinjectDependencyResolver : DependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver() : this(new StandardKernel()) { }

        public NinjectDependencyResolver(IKernel kernel) => _kernel = kernel;

        protected override void CopySingleton<TOriginal, TDestination>() => 
            _kernel.Bind<TDestination>().ToMethod(ctx => ctx.Kernel.Get<TOriginal>()).InSingletonScope();

        protected override IEnumerable<object> GetAll(Type type) => 
            _kernel.GetAll(type);

        protected override bool IsRegistered<TInterface>() => 
            _kernel.GetBindings(typeof(TInterface))?.Any() ?? false;

        protected override void RegisterSingleton<TInterface, TImplementation>() => 
            _kernel.Bind<TInterface>().To<TImplementation>().InSingletonScope();

        protected override void RegisterTransient<TInterface, TImplementation>() => 
            _kernel.Bind<TInterface>().To<TImplementation>().InTransientScope();

        protected override void ReleaseAll<T>()
        {
            var instances = _kernel.GetAll<T>();
            foreach(var instance in instances)
            {
                _kernel.Release(instance);
            }
        }

        protected override T Single<T>() => 
            _kernel.Get<T>();
    }
}