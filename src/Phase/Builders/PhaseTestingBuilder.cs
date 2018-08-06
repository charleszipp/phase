using Phase.Providers;
using Phase.Publishers;
using System;
using System.Collections.Generic;

namespace Phase.Builders
{
    /// <summary>
    /// Builder that initializes for unit testing.
    /// </summary>
    /// <remarks>
    /// This will engage an event publisher that publishes each event twice to ensure handlers are idempotent.
    /// </remarks>
    public class PhaseTestingBuilder : IPhaseBuilder
    {
        private readonly IEventsProvider _eventsProvider;
        private readonly DependencyResolver _resolver;
        private readonly Func<string, IDictionary<string, string>> _tenantKeysFactory;
        private readonly EventPublisher _publisher;

        public PhaseTestingBuilder(DependencyResolver resolver, IEventsProvider eventsProvider, Func<string, IDictionary<string, string>> tenantKeysFactory)
        {
            _resolver = resolver;
            _eventsProvider = eventsProvider;
            _tenantKeysFactory = tenantKeysFactory;
            _publisher = new IdemptotentTestingEventPublisher(resolver);
        }

        public Phase Build() => new Phase(_resolver, _eventsProvider, _tenantKeysFactory, _publisher);
    }
}