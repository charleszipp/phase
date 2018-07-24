using Phase.Domains;
using Phase.Interfaces;
using Phase.Tests.Events;
using System;

namespace Phase.Tests.Models
{
    public class MockReadModel : IHandleEvent<MockCreated>, IVolatileState
    {
        public DateTimeOffset CreateDate { get; private set; }

        public string Name { get; private set; }

        public void Handle(MockCreated e)
        {
            Name = e.MockName;
            CreateDate = e.Timestamp;
        }
    }
}