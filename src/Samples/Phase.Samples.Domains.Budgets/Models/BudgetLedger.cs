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
            //this operation is not idempotent. this will fail tests when using PhaseTestingBuilder
            //Accounts.Add(@event.AccountId, new AccountEntity(@event.AccountId, @event.AccountNumber, @event.AccountName));

            //this is example of the idempotent version of the operation add. This will pass despite the event being run twice.
            Accounts[@event.AccountId] = new AccountEntity(@event.AccountId, @event.AccountNumber, @event.AccountName);
        }
    }
}