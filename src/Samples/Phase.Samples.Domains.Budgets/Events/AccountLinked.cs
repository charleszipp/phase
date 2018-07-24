using Phase.Interfaces;
using System;
using System.Runtime.Serialization;

namespace Phase.Samples.Domains.Budgets.Events
{
    [DataContract]
    internal class AccountLinked : Event
    {
        internal AccountLinked(Guid accountId, string accountNumber, string accountName)
        {
            AccountId = accountId;
            AccountNumber = accountNumber;
            AccountName = accountName;
        }

        [DataMember]
        internal Guid AccountId { get; private set; }

        [DataMember]
        internal string AccountName { get; private set; }

        [DataMember]
        internal string AccountNumber { get; private set; }
    }
}