using System;
using System.Runtime.Serialization;

namespace Phase.Interfaces
{
    [DataContract]
    public abstract class Event : IEvent
    {
        public Event()
        {
            Type = GetType().AssemblyQualifiedName;
        }

        [DataMember]
        public string AggregateId { get; set; }

        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember]
        public string SequenceId { get; set; }

        [DataMember]
        public DateTimeOffset Timestamp { get; set; }

        [DataMember(Name = "_evtype")]
        public string Type { get; private set; }

        [DataMember]
        public int Version { get; set; }
    }
}