using System;

namespace Phase.Commands
{
    public interface IEvent
    {
        bool IsVolatile { get; set; }

        Guid EntityId { get; set; }

        string AggregateId { get; set; }

        string Id { get; set; }

        string Type { get; }

        int Version { get; set; }

        DateTimeOffset Timestamp { get; set; }

        string SequenceId { get; set; }
    }
}