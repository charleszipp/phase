using Phase.Builders;
using Phase.Ninject;
using Phase.Providers.Memory;
using Phase.Samples.Domains.Budgets;
using Phase.SpecFlowPlugin;
using System.Collections.Generic;

namespace Phase.Tests
{
    public static class ScenarioPhaseFactory
    {
        [ScenarioPhaseFactory]
        public static Phase ScenarioFactory()
        {
            var dependencyResolver = new NinjectDependencyResolver();
            var eventsProvider = new InMemoryEventsProvider(new InMemoryEventCollection(), TenantKeyFactory);
            return new PhaseBuilder(dependencyResolver, eventsProvider, TenantKeyFactory)
                .WithBudgets()
                .Build();
        }

        private static IDictionary<string, string> TenantKeyFactory(string tenantInstanceName) =>
            new Dictionary<string, string> { { "boardid", tenantInstanceName } };
    }
}