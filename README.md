# Phase
Phase is an in-process, platform agnostic, tenant isolating, CQRS & Event Sourcing for stateful services. Phase operates against in-memomry state as its primary source of data for both writes and reads. It then provides an abstraction for persisting the events to a durable store. This allows the entire framework to be executed end-to-end, in-proc from any platform. It also allows it to serve both commands and queries in sub-millisecond times

# Bootstrapping
Phase must be constructed of 3 dependent objects.
- Dependency Resolver - IoC container abstraction
- Events Provider - Persistence layer abstraction
- Tenant Key Factory - Delegate resolver for segments of a tenant key


The following example wires up Phase with the Ninject dependency resolver provided with `Phase.Ninject` package and the In-Memory Events Provider provided with `Phase` package

```csharp
var dependencyResolver = new NinjectDependencyResolver();
var eventsProvider = new InMemoryEventsProvider(new InMemoryEventCollection(), TenantKeyFactory);
var phase = new PhaseBuilder(dependencyResolver, eventsProvider, TenantKeyFactory)
    .Build();
```

Phase provides a builder/decorator implementation to extend the construction of phase. Out-of-the-box the `Phase` package provides several builders for wiring up command and query handlers, aggregates, and read models

```csharp
var dependencyResolver = new NinjectDependencyResolver();
var tenantKeyFactory = (tenantInstanceName) => new Dictionary<string, string> { { "boardid", tenantInstanceName } };
var eventsProvider = new InMemoryEventsProvider(new InMemoryEventCollection(), tenantKeyFactory);
var phase = new PhaseBuilder(dependencyResolver, eventsProvider, tenantKeyFactory)
    .WithCommandHandler<LinkAccountHandler, LinkAccount>()
    .WithQueryHandler<GetAccountsHandler, GetAccounts, GetAccountsResult>()
    .WithAggregateRoot<BudgetAggregate>()
    .WithReadModel<BudgetLedger>()
    .WithStatefulEventSubscriber<AccountLinked, BudgetLedger>()
    .Build();
```

These can be further abstracted into their own decorator to encapsulate dependencies for a given domain module

```csharp
var dependencyResolver = new NinjectDependencyResolver();
var tenantKeyFactory = (tenantInstanceName) => new Dictionary<string, string> { { "boardid", tenantInstanceName } };
var eventsProvider = new InMemoryEventsProvider(new InMemoryEventCollection(), tenantKeyFactory);
var phase = new PhaseBuilder(dependencyResolver, eventsProvider, tenantKeyFactory)
    .WithBudgets() // new decorator that encapsulates the commands, queries, and models
    .Build();
```

# Occupying/Vacating Phase
Phase essentially operates as a stateful actor within a system. Before any commands or queries can be executed, phase must first be occupied by a tenant. Once occupied, the commands and queries will be tenant context aware and be able to provide tenant isolation.

**IMPORTANT!** 
> Occupy/Vacate is not something that should be performed in a stateless api/service on a per request basis. This is intended to be used within a stateful object that is started under a given context and serve mulitiple requests under that context. Examples of this would be Virtual Actors (from Orleans and Azure Service Fabric) and Stateful Reliable Services (from Azure Service Fabric).

Within a platform's startup/activation phase should be occupied for a given tenant. 

```csharp
await phase.OccupyAsync(tenantId, cancellationToken);
```

Occupy will prime the in-memory state. It does this by attempting to pull all events from the Events Provider for the givent tenant and replay those events on the Read Model or any other event subscribers.

Once commands and queries are no longer needed to be served for a given tenant, the Vacate method should be invoked. This will dehydrate all in-memory state associated with the given tenant

```csharp
await phase.VacateAsync(cancellationToken)
```

# Putting It All Together
```csharp
var budgetId = Guid.NewGuid().ToString();
var cancellation = new CancellationTokenSource();

var dependencyResolver = new NinjectDependencyResolver();
var tenantKeyFactory = (tenantInstanceName) => new Dictionary<string, string> { { "boardid", tenantInstanceName } };
var eventsProvider = new InMemoryEventsProvider(new InMemoryEventCollection(), tenantKeyFactory);
var phase = new PhaseBuilder(dependencyResolver, eventsProvider, tenantKeyFactory)
    .WithCommandHandler<LinkAccountHandler, LinkAccount>()
    .WithQueryHandler<GetAccountsHandler, GetAccounts, GetAccountsResult>()
    .WithAggregateRoot<BudgetAggregate>()
    .WithReadModel<BudgetLedger>()
    .WithStatefulEventSubscriber<AccountLinked, BudgetLedger>()
    .Build();

// occupy for the budget aggregate/tenant
await phase.OccupyAsync(budgetId, cancellation.Token);

// execute a command to add a new account
await phase.ExecuteAsync(new LinkAccount(Guid.NewGuid(), "111", "Checking"), cancellation.Token);

// execute a query, get the accounts
var result = await phase.QueryAsync(new GetAccounts(), cancellation.Token);

// vacate for this tenant
await phase.VacateAsync(cancellation.Token);
```

# Defining the Constructs
The following will illustrate how the different constructs within phase can be implemented. There are 4 major constructs
- Commands
- Queries
- Aggregate Root (aka Write Model)
- Read Model

## Commands
Commands should execute an operation whose intent is to potentially modify state. Commands should use an Aggregate Root to perform validation and raise events.

> **Important!**
> Commands should never use the Read Model! Always use an aggregate root for data needed for validation or constructing new events.

Commands are implemented by defining a new class that inherits from the `CommandHandler<T>` abstraction. This abstraction expects on `Execute` method be implemented. The execute method should encapsulate the validation and raising one or more new events.

```csharp
internal class LinkAccountHandler : CommandHandler<LinkAccount>
{
    public override async Task Execute(LinkAccount command, CancellationToken cancellationToken)
    {
        // get the aggregate root
        var budget = await GetAggregateAsync<BudgetAggregate>(TenantId, cancellationToken);

        // perform validation
        if(budget.Accounts.Any(a => (a.Key == command.AccountId) || (a.Value.Name == command.AccountNumber && a.Value.Number == command.AccountNumber)))
            throw new ArgumentException("Account already exists with the same name and number or id");

        // raise an event
        await RaiseEventAsync<BudgetAggregate>(TenantId, new AccountLinked(command.AccountId, command.AccountNumber, command.AccountName), cancellationToken);
    }
}
```

# More docs coming soon...
