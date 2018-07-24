using MongoDB.Bson;
using Phase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Phase.Domains
{
    public class AggregateDescriptor
    {
        public AggregateDescriptor(string aggregateId, AggregateRoot aggregateRoot)
        {
            AggregateId = aggregateId;
            AggregateRoot = aggregateRoot;
            EntityId = Guid.Parse(aggregateId.Substring(aggregateId.LastIndexOf('/') + 1));
        }

        private readonly List<IEvent> _changes = new List<IEvent>();

        public AggregateRoot AggregateRoot { get;}

        public string AggregateId { get; }

        public Guid EntityId { get; set; }

        public int Version { get; private set; }

        public IEvent[] Flush()
        {
            lock (_changes)
            {
                var changes = _changes.ToList();
                var i = 0;
                foreach (var @event in changes)
                {
                    i++;
                    @event.Version = Version + i;
                    @event.Id = Guid.NewGuid().ToString();
                }
                Version = Version + _changes.Count;
                _changes.Clear();
                return changes.ToArray();
            }
        }

        public void Replay(IEnumerable<IEvent> eventHistory)
        {
            lock (_changes)
            {
                foreach (var e in eventHistory.OrderBy(e => e.Version))
                {
                    OnApply(e);
                    Version = e.Version;
                }
            }
        }

        public void Apply(params IEvent[] events)
        {
            lock (_changes)
            {
                foreach (var @event in events)
                {
                    @event.SequenceId = ObjectId.GenerateNewId().ToString();
                    @event.Timestamp = DateTimeOffset.UtcNow;
                    if (@event.EntityId == Guid.Empty)
                        @event.EntityId = EntityId;
                    if (string.IsNullOrEmpty(@event.AggregateId))
                        @event.AggregateId = AggregateId;
                    OnApply(@event);
                    _changes.Add(@event);
                }
            }
        }

        private void OnApply(IEvent @event)
        {
            bool hasApply = AggregateRoot.GetType().GetMethod("Apply",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new Type[] { @event.GetType() },
                null) != null;

            if (hasApply)
            {
                dynamic a = AggregateRoot;
                dynamic e = @event;
                a.Apply(e);
            }
        }
    }
}