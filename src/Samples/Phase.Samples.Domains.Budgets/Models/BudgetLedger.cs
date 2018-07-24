using Phase.Domains;
using Phase.Interfaces;
using Phase.Samples.Domains.Budgets.Events;

namespace Phase.Samples.Domains.Budgets.Models
{
    internal class BudgetLedger : IHandleEvent<AccountLinked>, IVolatileState
    {
        //todo setup ledger model and handle event...
        public void Handle(AccountLinked @event)
        {
        }
    }
}