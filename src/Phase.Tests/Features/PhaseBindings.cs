using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phase.Builders;
using Phase.Ninject;
using Phase.Providers.Memory;
using Phase.Tests.Commands;
using Phase.Tests.Events;
using Phase.Tests.Models;
using Phase.Tests.Queries;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Assist;

namespace Phase.Tests.Features
{
    [Binding]
    public class PhaseBindings
    {
        private readonly CancellationTokenSource _cancellation;
        private readonly Phase _phase;

        public PhaseBindings()
        {
            var dependencyResolver = new NinjectDependencyResolver();
            var eventsProvider = new InMemoryEventsProvider(new InMemoryEventCollection(), TenantKeyFactory);

            _phase = new PhaseBuilder(dependencyResolver, eventsProvider, TenantKeyFactory)
                .WithCommandHandler<CreateMockHandler, CreateMock>()
                .WithCommandHandler<LinkAccountHandler, LinkAccount>()
                .WithQueryHandler<GetMockHandler, GetMock, GetMockResult>()
                .WithAggregateRoot<MockAggregate>()
                .WithReadModel<MockReadModel>()
                .WithAggregateRoot<BudgetAggregate>()
                .WithReadModel<BudgetLedger>()
                .WithStatefulEventSubscriber<MockCreated, MockReadModel>()
                .WithStatefulEventSubscriber<AccountLinked, BudgetLedger>()
                .Build();
            _cancellation = new CancellationTokenSource();
        }

        [AfterStep("CatchException")]
        public void CatchException()
        {
            if (ScenarioContext.Current.StepContext.StepInfo.StepDefinitionType == StepDefinitionType.When)
            {
                PropertyInfo testStatusProperty = typeof(ScenarioContext).GetProperty(nameof(ScenarioContext.Current.ScenarioExecutionStatus), BindingFlags.Public | BindingFlags.Instance);
                testStatusProperty.SetValue(ScenarioContext.Current, ScenarioExecutionStatus.OK);
            }
        }

        [Given(@"phase is vacant")]
        public async Task GivenThePhaseClientIsVacant()
        {
            if (_phase.State == States.PhaseStates.Occupied)
                await _phase.VacateAsync(_cancellation.Token);
        }

        [Given(@"phase is occupied with tenant id ""(.*)""")]
        [When(@"occupy phase with tenant id ""(.*)""")]
        public Task OccupyPhaseWithTenantId(string tenantId) =>
            _phase.OccupyAsync(tenantId, _cancellation.Token);

        [Then(@"an exception should be thrown with message ""(.*)""")]
        public void ThenAnExceptionShouldBeThrownWithMessage(string exceptionMessage)
        {
            Assert.AreEqual(exceptionMessage, ScenarioContext.Current.TestError?.Message);
        }

        [Then(@"phase should be occupied with tenant id ""(.*)""")]
        public void ThenPhaseShouldBeOccupiedWithTenantId(string tenantId)
        {
            Assert.AreEqual(States.PhaseStates.Occupied, _phase.State);
            Assert.AreEqual(tenantId, _phase.TenantId);
        }

        [Then(@"phase should be vacant")]
        public void ThenPhaseShouldBeVacant()
        {
            Assert.AreEqual(States.PhaseStates.Vacant, _phase.State);
            Assert.IsNull(_phase.TenantId);
        }

        [When(@"vacate phase")]
        public Task VacatePhase() =>
            _phase.VacateAsync(_cancellation.Token);

        [When(@"executing a command without result")]
        public Task WhenExecutingACommandWithoutResult() =>
            _phase.ExecuteAsync(new CreateMock("Mock 1"), _cancellation.Token);

        [When(@"executing a query")]
        public Task WhenExecutingAQuery() =>
            _phase.QueryAsync(new GetMock(), _cancellation.Token);

        [When(@"phase executes create mock command")]
        public Task WhenPhaseExecutesCreateMockCommand(Table table) => 
            _phase.ExecuteAsync(table.CreateImmutableInstance<CreateMock>(), _cancellation.Token);

        [When(@"phase executes get mock query")]
        public async Task WhenPhaseExecutesGetMockQuery()
        {
            var result = await _phase.QueryAsync(new GetMock(), _cancellation.Token);
            ScenarioContext.Current["GetMockResult"] = result;
        }

        [Then(@"the query should return mock name ""(.*)""")]
        public void ThenTheQueryShouldReturnMockName(string expectedMockName)
        {
            GetMockResult result = (GetMockResult)ScenarioContext.Current["GetMockResult"];
            Assert.AreEqual(expectedMockName, result.MockName);
        }


        private IDictionary<string, string> TenantKeyFactory(string tenantInstanceName) =>
            new Dictionary<string, string> { { "boardid", tenantInstanceName } };
    }
}