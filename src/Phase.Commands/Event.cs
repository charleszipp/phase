using System;
using System.Runtime.Serialization;

namespace Phase.Commands
{
    [DataContract]
    public abstract class Event : IEvent
    {
        public Event()
        {
            Type = GetType().AssemblyQualifiedName;
        }

        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public DateTimeOffset Timestamp { get; set; }

        [DataMember]
        public string  SequenceId { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember]
        public string AggregateId { get; set; }

        [DataMember(Name = "_evtype")]
        public string Type { get; private set; }
        
        /// <summary>
        /// Indicates if the event is commited (volatile = false) or uncommitted (volatile = true)
        /// </summary>
        /// <remarks>
        /// This should not need to be serialized to document storage. If its getting to storage 
        /// then its committed which is represented by its default value (volatile = false).
        /// </remarks>
        public bool IsVolatile { get; set; } = false;
    }
}