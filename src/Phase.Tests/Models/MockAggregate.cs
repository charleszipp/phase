using Phase.Domains;
using Phase.Tests.Events;
using System;

namespace Phase.Tests.Models
{
    [PhaseAggregate("mocks")]
    public class MockAggregate : AggregateRoot
    {
        public DateTimeOffset CreateDate { get; private set; }

        public string Name { get; private set; }

        public void Apply(MockCreated e)
        {
            Name = e.MockName;
            CreateDate = e.Timestamp;
        }
    }
}