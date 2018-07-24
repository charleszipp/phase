using Phase.Domains;
using System;
using System.Collections.Generic;

namespace Phase.Samples.Domains.Budgets.Models
{
    [PhaseAggregate("budgets")]
    internal class BudgetAggregate : AggregateRoot, IVolatileState
    {
        internal IDictionary<Guid, AccountEntity> Accounts { get; } = new Dictionary<Guid, AccountEntity>();
    }
}