using Phase.Domains;
using Phase.Tests.Events;
using Phase.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Tests.Commands
{
    public class LinkAccountHandler : CommandHandler<LinkAccount>
    {
        public override async Task Execute(LinkAccount command, CancellationToken cancellationToken)
        {
            var budget = await GetAggregateAsync<BudgetAggregate>(TenantId, cancellationToken);

            if(budget.Accounts.Any(a => (a.Key == command.AccountId) || (a.Value.Name == command.AccountNumber && a.Value.Number == command.AccountNumber)))
                throw new ArgumentException("Account already exists with the same name and number or id");

            await RaiseEventAsync<BudgetAggregate>(TenantId, new AccountLinked(command.AccountId, command.AccountNumber, command.AccountName), cancellationToken);
        }
    }
}
