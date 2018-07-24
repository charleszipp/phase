using Phase.Samples.Domains.Budgets.Interfaces.Models;
using System.Collections.Generic;

namespace Phase.Samples.Domains.Budgets.Interfaces.Queries
{
    public class GetAccountsResult
    {
        public GetAccountsResult(IEnumerable<Account> accounts)
        {
            Accounts = accounts;
        }

        public IEnumerable<Account> Accounts { get; }
    }
}
