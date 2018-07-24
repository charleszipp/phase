using Phase.Domains;
using Phase.Interfaces;
using Phase.Mediators;
using Phase.Providers;
using Phase.Publishers;
using Phase.States;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Phase
{
    public sealed class Phase : ITenantContext
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private PhaseState _currentState = new VacantState();

        internal Phase(DependencyResolver resolver, IEventsProvider eventsProvider, Func<string, IDictionary<string, string>> tenantKeysFactory)
        {
            DependencyResolver = resolver;
            EventsProvider = eventsProvider;
            Mediator = new Mediator(resolver);
            Session = new Session(resolver, EventsProvider);
            resolver._session = Session;
            Publisher = new EventPublisher(resolver);
        }

        public DependencyResolver DependencyResolver { get; }

        public PhaseStates State { get; internal set; } = PhaseStates.Vacant;

        public string TenantId { get; internal set; }

        internal IEventsProvider EventsProvider { get; }

        internal Mediator Mediator { get; }

        internal EventPublisher Publisher { get; }

        internal Session Session { get; }

        public async Task ExecuteAsync(ICommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
                await _currentState.ExecuteAsync(this, command, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task OccupyAsync(string tenantId, CancellationToken cancellationToken)
        {
            try
            {
                await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
                _currentState = await _currentState.OccupyAsync(this, tenantId, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            try
            {
                await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
                return await _currentState.QueryAsync(this, query, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task VacateAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
                _currentState = await _currentState.VacateAsync(this, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}