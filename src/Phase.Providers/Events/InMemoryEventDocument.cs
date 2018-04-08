using Phase.Commands;
using System.Collections.Generic;

namespace Phase.Providers
{
    public class InMemoryEventDocument
    {
        public InMemoryEventDocument(IDictionary<string, string> keys, IEvent @event)
        {
            Keys = keys;
            Event = @event;
        }

        public IDictionary<string, string> Keys { get; }

        public IEvent Event { get; }
    }
}