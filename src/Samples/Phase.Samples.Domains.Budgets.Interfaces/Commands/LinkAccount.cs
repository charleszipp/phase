using Phase.Interfaces;
using System;

namespace Phase.Samples.Domains.Budgets.Interfaces.Commands
{
    public class LinkAccount : ICommand
    {
        public LinkAccount(Guid accountId, string accountNumber, string accountName)
        {
            AccountId = accountId;
            AccountNumber = accountNumber;
            AccountName = accountName;
        }

        public Guid AccountId { get; }

        public string AccountName { get; }

        public string AccountNumber { get; }
    }
}