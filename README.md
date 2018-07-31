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

## Aggregate Roots
The aggregate root represents a typical aggregate as defined in the Domain Driven Design concepts. It represents the domain's model and boundary. Its the only object in the domain model that the commands can/should reference directly. The data within aggegates are considered to be immediately consistent. This makes it safe for commands to use for validation and making decisions.

Aggregates should inherit from the AggregateRoot base class. They should also require the `PhaseAggegate` attribute. This attribute defines a segment that will be used in URI's generated for all events raised against the aggregate.

Aggregates are stateful objects. They should define a data model for the data they maintain. The model should be tailored for command validation efficiency.

Aggregates handle raised events. To handle an event a method should be defined with the following signature. The runtime will execute this method immediately when an event is raised against the aggregate. This enables the command to immediately read the results of the event it raised. 

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

> **Important!**
> Events should be the only source of data used to populate an Aggregate's data model. When the Aggregate is rehydrated in a subsequent request, these events will be replayed to rebuild the data model.

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
### Important Assumptions
When building the read model, these 3 assumptions should be kept in mind.

#### Eventual Consistency
The state is eventually consistent with respect to the Aggregate Root. Therefore, these are not safe to use within commands.

#### Guaranteed Order
Events will always be played in the order they were raised. Therefore, the event handlers can make assumptions about the order in which they are expected to be executed.

#### At Least Once Delivery
Events are guaranteed to be delivered at least once. This implies that a single event may be delivered multiple times. This may occur in the event of a transient failure. Therefore, the `Handle` methods of the read model should be idempotent. 

> Coming Soon: Builder extension to configure phase to publish all events twice. This would be used from within only the unit tests to ensure handlers are kept idempotent.

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

## Queries
Queries should execute and operation whose intent is to only return data. Queries should not modify state. Queries should use a Read Model to materialize the data being requested.

Queries are implemented by defining a new class that implements IHandleQuery<TQuery, TResult>. TQuery represents the DTO that contains the arguments needed to fulfil the query. TResult is the DTO that represents the result of the query. The execute method should encapsulate the steps needed to fulfil the query

> Note
> The read model can be injected via constructor injection. The `WithReadModel` builder extension will register the instance with the provided dependency resolver making it possible to inject. This is true for any of the object registered with the builder extensions!

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

> **Important!**
> Always enumerate lists that are returned from a query handler. Never return an enumerable that holds a reference back to the read model. Executing `ToList` or `ToArray` should do the trick in most cases. Deep copy can also be used if necessary.