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

# Occupying/Vacating Phase
Phase essentially operates as a stateful actor within a system. Before any commands or queries can be executed, phase must first be occupied by a tenant. Once occupied, the commands and queries will be tenant context aware and be able to provide tenant isolation.

Within a platform's startup/activation phase should be occupied for a given tenant. 
```csharp
await phase.OccupyAsync(tenantId, cancellationToken);
```
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

## Aggregate Roots
The aggregate root represents a typical aggregate as defined in the Domain Driven Design concepts. It represents the domain's model and boundary. Its the only object in the domain model that the commands can/should reference directly. The data within aggegates are considered to be immediately consistent. This makes it safe for commands to use for validation and making decisions.

```csharp
[PhaseAggregate("budgets")]
internal class BudgetAggregate : AggregateRoot
{
    // define the state this aggregate uses
    internal IDictionary<Guid, AccountEntity> Accounts { get; } = new Dictionary<Guid, AccountEntity>();

    // define events that it handles
    public void Apply(AccountLinked e)
    {
        Accounts[e.AccountId] = new AccountEntity(e.AccountId, e.AccountNumber, e.AccountName);
    }
}
```

## Read Models
The read model is typically an alternate representation of the domain model structured for read efficiency. For example, if the application typically serves aggregated/summarized data disproportionately more often than it takes input from users then it can be beneficial to create a denormalized model tailored for aggregation/summarization. An example of such a model would be one that follows the traditional star or snowflake schema.

Read models are stateful objects. Their source of data should also be the events. Read models can subscribe to events by implementing the `IHandleEvent<TEvent>` interface. Then registered via the builder extension `WithStatefulEventSubscriber<TEvent, TEventHandler>()`

Read models are hydrated during `OccupyAsync`. The event stream is replayed on the read model to bring it back to the last known state of the system.

```csharp
internal class BudgetLedger : IHandleEvent<AccountLinked>, IVolatileState
{
    internal IDictionary<Guid, AccountEntity> Accounts = new Dictionary<Guid, AccountEntity>();

    public void Handle(AccountLinked @event)
    {
        Accounts[@event.AccountId] = new AccountEntity(@event.AccountId, @event.AccountNumber, @event.AccountName);
    }
}
```
## Commands
Commands should execute an operation whose intent is to potentially modify state. Commands should use an Aggregate Root to perform validation and raise events.
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

## Queries
Queries should execute and operation whose intent is to only return data. Queries should not modify state. Queries should use a Read Model to materialize the data being requested.
```csharp
internal class GetAccountsHandler : IHandleQuery<GetAccounts, GetAccountsResult>
{
    // read model
    private readonly BudgetLedger _ledger;

    // inject the read model
    public GetAccountsHandler(BudgetLedger ledger)
    {
        _ledger = ledger;
    }

    public Task<GetAccountsResult> Execute(GetAccounts query, CancellationToken cancellationToken)
    {
        // execute a linq to objects query against the read model to materialize and return the result
        return Task.FromResult(new GetAccountsResult(
            _ledger.Accounts.Select(a => new Account(a.Key, a.Value.Number, a.Value.Name)).ToList() // be sure to enumerate!
            ));
    }
}
```
