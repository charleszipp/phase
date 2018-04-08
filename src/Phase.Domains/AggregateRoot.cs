using Phase.Commands;
using Phase.Providers;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Phase.Domains
{
    public class AggregateRoot : MarshalByRefObject
    {
        public static string GetAggregateKeyFromEntityId<T>(Guid entityId)
        {
            var aggregateType = typeof(T);
            var aggAttribute = aggregateType.GetCustomAttribute<AggregateAttribute>(false);
            if (aggAttribute == null)
                throw new Exception($"Aggregate type of {aggregateType.FullName} does not define an aggregate name. Use AggregateAttribute to define the aggregate name.");
            var uri = "/" + string.Join("/", aggAttribute.AggregateName, entityId);
            return new Uri(uri, UriKind.Relative).ToString();
        }

        public static Guid GetEntityIdFromAggregateId(string aggregateId)
        {
            return Guid.Parse(aggregateId.Substring(aggregateId.LastIndexOf('/') + 1));
        }

        private readonly List<IEvent> _changes = new List<IEvent>();

        public string AggregateId { get; protected set; }

        public Guid EntityId { get; protected set; }

        public int Version { get; protected set; }

        public IEventsPublisher EventsPublisher { get; protected set; }

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
                    @event.IsVolatile = false;
                }
                Version = Version + _changes.Count;
                _changes.Clear();
                return changes.ToArray();
            }
        }

        public void Load(IEventsPublisher publisher, IEnumerable<IEvent> history, string aggregateId)
        {
            EventsPublisher = publisher;
            lock (_changes)
            {
                AggregateId = aggregateId;
                EntityId = GetEntityIdFromAggregateId(aggregateId);
                foreach (var e in history.OrderBy(e => e.Version))
                {
                    OnApply(e);
                    Version = e.Version;
                }
            }
        }

        public Task RaiseEventAsync(IEvent @event)
        {
            OnApplyEvents(@event);
            return EventsPublisher.PublishAsync(@event);
        }

        private void OnApplyEvents(params IEvent[] events)
        {
            lock (_changes)
            {
                foreach(var @event in events)
                {
                    @event.IsVolatile = true;
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
            bool hasApply = GetType().GetMethod("Apply",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new Type[] { @event.GetType() },
                null) != null;

            if (hasApply)
            {
                dynamic a = this;
                dynamic e = @event;
                a.Apply(e);
            }
        }
    }
}