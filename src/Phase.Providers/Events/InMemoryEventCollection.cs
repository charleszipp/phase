﻿using Phase.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Phase.Providers
{
    public class InMemoryEventCollection
    {
        private ConcurrentDictionary<string, IEnumerable<InMemoryEventDocument>> _db = new ConcurrentDictionary<string, IEnumerable<InMemoryEventDocument>>();

        public void AddOrUpdateEvents(IEnumerable<IEvent> events, IDictionary<string, string> serviceInstanceKeys)
        {
            var eventsById = events
                .Select(e => new InMemoryEventDocument(serviceInstanceKeys, e))
                .GroupBy(doc => doc.Event.AggregateId);

            foreach (var e in eventsById)
            {
                _db.AddOrUpdate(e.Key, e, (key, existingEvents) =>
                {
                    var newEvents = new List<InMemoryEventDocument>(existingEvents);
                    newEvents.AddRange(e);
                    return newEvents;
                });
            }
        }

        public IEnumerable<IEvent> Get(string aggregateId, int fromVersion = -1)
        {
            IEnumerable<IEvent> rvalues = new List<IEvent>();

            if (_db.ContainsKey(aggregateId))
                rvalues = _db[aggregateId]
                    .Where(doc => doc.Event.Version > fromVersion)
                    .Select(doc => doc.Event)
                    .ToList();

            return rvalues;
        }

        public IEnumerable<IEvent> Get(IDictionary<string, string> serviceInstanceKeys)
        {
            return _db
                .SelectMany(doc => doc.Value)
                .Where(doc => doc.Keys.SequenceEqual(serviceInstanceKeys))
                .Select(doc => doc.Event)
                .ToList();
        }
    }
}