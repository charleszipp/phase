using Phase.Interfaces;
using System;
using System.Runtime.Serialization;

namespace Phase.Tests.Events
{
    [DataContract]
    public class MockCreated : Event
    {
        public MockCreated(string mockName)
        {
            MockName = mockName;
        }

        [DataMember]
        public string MockName { get; }
    }
}