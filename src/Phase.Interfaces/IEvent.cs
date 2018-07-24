using System;

namespace Phase.Interfaces
{
    public interface IEvent
    {
        string AggregateId { get; set; }

        Guid EntityId { get; set; }

        string Id { get; set; }

        string SequenceId { get; set; }

        DateTimeOffset Timestamp { get; set; }

        string Type { get; }

        int Version { get; set; }
    }
}