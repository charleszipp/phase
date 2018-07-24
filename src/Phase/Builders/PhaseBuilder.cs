using Phase.Providers;
using System;
using System.Collections.Generic;

namespace Phase.Builders
{
    public class PhaseBuilder : IPhaseBuilder
    {
        private readonly IEventsProvider _eventsProvider;
        private readonly DependencyResolver _resolver;
        private readonly Func<string, IDictionary<string, string>> _tenantKeysFactory;

        public PhaseBuilder(DependencyResolver resolver, IEventsProvider eventsProvider, Func<string, IDictionary<string, string>> tenantKeysFactory)
        {
            _resolver = resolver;
            _eventsProvider = eventsProvider;
            _tenantKeysFactory = tenantKeysFactory;
        }

        public Phase Build() => new Phase(_resolver, _eventsProvider, _tenantKeysFactory);
    }
}