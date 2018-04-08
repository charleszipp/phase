using System;
using System.Collections.Generic;

namespace Phase.Providers
{
    public class InMemoryEventsProviderConfiguration
    {
        public InMemoryEventsProviderConfiguration(Func<string, IDictionary<string, string>> serviceInstanceKeysFactory)
        {
            ServiceInstanceKeysFactory = serviceInstanceKeysFactory;
        }

        public Func<string, IDictionary<string, string>> ServiceInstanceKeysFactory { get; }
    }
}