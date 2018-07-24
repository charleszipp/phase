using Phase.Interfaces;
using System;
using System.Runtime.Serialization;

namespace Phase.Tests.Events
{
    [DataContract]
    public class AccountLinked : Event
    {
        public AccountLinked(Guid accountId, string accountNumber, string accountName)
        {
            AccountId = accountId;
            AccountNumber = accountNumber;
            AccountName = accountName;
        }

        [DataMember]
        public Guid AccountId { get; private set; }

        [DataMember]
        public string AccountName { get; private set; }

        [DataMember]
        public string AccountNumber { get; private set; }
    }
}