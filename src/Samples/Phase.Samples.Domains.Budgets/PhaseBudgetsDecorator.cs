using Phase.Builders;
using Phase.Samples.Domains.Budgets.Commands;
using Phase.Samples.Domains.Budgets.Events;
using Phase.Samples.Domains.Budgets.Handlers;
using Phase.Samples.Domains.Budgets.Interfaces.Commands;
using Phase.Samples.Domains.Budgets.Interfaces.Queries;
using Phase.Samples.Domains.Budgets.Models;

namespace Phase.Samples.Domains.Budgets
{
    public static class PhaseBudgetsFluentDecorator
    {
        public static IPhaseBuilder WithBudgets(this IPhaseBuilder builder)
        {
            return new PhaseBudgetsDecorator(builder);
        }
    }

    public class PhaseBudgetsDecorator : PhaseDecorator
    {
        public PhaseBudgetsDecorator(IPhaseBuilder builder)
            : base(builder)
        {
        }

        public override Phase Build()
        {
            return _builder
                .WithCommandHandler<LinkAccountHandler, LinkAccount>()
                .WithQueryHandler<GetAccountsHandler, GetAccounts, GetAccountsResult>()
                .WithAggregateRoot<BudgetAggregate>()
                .WithReadModel<BudgetLedger>()
                .WithStatefulEventSubscriber<AccountLinked, BudgetLedger>()
                .Build();
        }
    }
}