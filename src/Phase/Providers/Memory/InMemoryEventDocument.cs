using Phase.Interfaces;
using System.Collections.Generic;

namespace Phase.Providers.Memory
{
    public class InMemoryEventDocument
    {
        public InMemoryEventDocument(IDictionary<string, string> keys, IEvent @event)
        {
            Keys = keys;
            Event = @event;
        }

        public IEvent Event { get; }

        public IDictionary<string, string> Keys { get; }
    }
}