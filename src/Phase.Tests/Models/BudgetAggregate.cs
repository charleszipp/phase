using Phase.Domains;
using System;
using System.Collections.Generic;

namespace Phase.Tests.Models
{
    public class BudgetAggregate : AggregateRoot, IVolatileState
    {
        public IDictionary<Guid, AccountEntity> Accounts { get; } = new Dictionary<Guid, AccountEntity>();
    }
}