using Phase.Interfaces;
using Phase.Samples.Domains.Budgets.Interfaces.Queries;
using Phase.Samples.Domains.Budgets.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Samples.Domains.Budgets.Handlers
{
    internal class GetAccountsHandler : IHandleQuery<GetAccounts, GetAccountsResult>
    {
        private readonly BudgetLedger _ledger;

        public GetAccountsHandler(BudgetLedger ledger)
        {
            _ledger = ledger;
        }

        public Task<GetAccountsResult> Execute(GetAccounts query, CancellationToken cancellationToken)
        {
            //todo get accounts from the ledger...
            throw new NotImplementedException();
        }
    }
}