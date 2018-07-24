using Phase.Interfaces;
using System;

namespace Phase.Tests.Commands
{
    public class LinkAccount : ICommand
    {
        public LinkAccount(Guid accountId, string accountNumer, string accountName)
        {
            AccountId = accountId;
            AccountNumber = accountNumer;
            AccountName = accountName;
        }

        public Guid AccountId { get; }

        public string AccountName { get; }

        public string AccountNumber { get; }
    }
}