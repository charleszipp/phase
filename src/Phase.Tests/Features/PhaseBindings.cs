﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phase.Samples.Domains.Budgets.Interfaces.Commands;
using Phase.Samples.Domains.Budgets.Interfaces.Models;
using Phase.Samples.Domains.Budgets.Interfaces.Queries;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Phase.Tests.Features
{
    [Binding]
    public class PhaseBindings
    {
        private readonly CancellationTokenSource _cancellation;
        private readonly Phase _phase;

        public PhaseBindings(Phase phase)
        {
            _phase = phase;
            _cancellation = new CancellationTokenSource();
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

        [Then(@"the query should return the following accounts")]
        public void ThenTheQueryShouldReturnMockName(Table table)
        {
            GetAccountsResult result = (GetAccountsResult)ScenarioContext.Current["GetAccountsResult"];
            var expectedAccounts = table.CreateImmutableSet<Account>();
            Assert.IsTrue(expectedAccounts.ToProjection(table).SequenceEqual(result.Accounts.ToProjection()));
        }

        [When(@"vacate phase")]
        public Task VacatePhase() =>
            _phase.VacateAsync(_cancellation.Token);

        [When(@"executing a command without result")]
        public Task WhenExecutingACommandWithoutResult() =>
            _phase.ExecuteAsync(new LinkAccount(Guid.NewGuid(), "111", "Checking"), _cancellation.Token);

        [When(@"executing a query")]
        public Task WhenExecutingAQuery() =>
            _phase.QueryAsync(new GetAccounts(), _cancellation.Token);

        [When(@"phase executes link account command")]
        public Task WhenPhaseExecutesCreateMockCommand(Table table) =>
            _phase.ExecuteAsync(table.CreateImmutableInstance<LinkAccount>(), _cancellation.Token);

        [When(@"phase executes get accounts query")]
        public async Task WhenPhaseExecutesGetMockQuery()
        {
            var result = await _phase.QueryAsync(new GetAccounts(), _cancellation.Token);
            ScenarioContext.Current["GetAccountsResult"] = result;
        }
    }
}