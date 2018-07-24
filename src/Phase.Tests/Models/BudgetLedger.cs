using Phase.Domains;
using Phase.Interfaces;
using Phase.Tests.Events;

namespace Phase.Tests.Models
{
    public class BudgetLedger : IHandleEvent<AccountLinked>, IVolatileState
    {
        public void Handle(AccountLinked @event)
        {
        }
    }
}