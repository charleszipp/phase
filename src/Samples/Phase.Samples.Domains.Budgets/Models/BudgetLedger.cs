using Phase.Domains;
using Phase.Interfaces;
using Phase.Samples.Domains.Budgets.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Phase.Samples.Domains.Budgets.Models
{
    internal class BudgetLedger : IHandleEvent<AccountLinked>, IVolatileState
    {
        internal IDictionary<Guid, AccountEntity> Accounts = new Dictionary<Guid, AccountEntity>();

        public void Handle(AccountLinked @event)
        {
            Accounts[@event.AccountId] = new AccountEntity(@event.AccountId, @event.AccountNumber, @event.AccountName);
        }
    }
}