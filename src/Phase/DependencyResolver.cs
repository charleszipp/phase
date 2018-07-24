using Phase.Domains;
using Phase.Interfaces;
using Phase.Publishers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Phase
{
    public abstract class DependencyResolver
    {
        internal Session _session { get; set; }

        protected abstract void RegisterTransient<TInterface, TImplementation>()
            where TImplementation : class, TInterface;

        protected abstract void RegisterSingleton<TInterface, TImplementation>() 
            where TImplementation : class, TInterface;

        protected abstract void CopySingleton<TOriginal, TDestination>()
            where TOriginal : TDestination;

        protected abstract T Single<T>();

        protected abstract IEnumerable<object> GetAll(Type type);

        protected abstract void ReleaseAll<T>();

        protected abstract bool IsRegistered<TInterface>();

        internal TAggregate GetAggregateRoot<TAggregate>() where TAggregate : AggregateRoot => 
            AggregateProxy<TAggregate>.Create(Single<TAggregate>());

        internal IHandleCommand<TCommand> GetCommandHandler<TCommand>(ITenantContext tenant) where TCommand : ICommand
        {
            var rvalue = Single<IHandleCommand<TCommand>>();
            InitializeCommandHandler(rvalue, tenant);
            return rvalue;
        }

        internal IEnumerable<EventSubscriber> GetEventSubscribers(IEvent @event)
        {
            var eventType = @event.GetType();
            var subscriberType = typeof(EventSubscriber<>).MakeGenericType(eventType);
            var handlerType = typeof(IHandleEvent<>).MakeGenericType(eventType);
            return GetAll(handlerType)
                .Select(handler => Activator.CreateInstance(subscriberType, handler))
                .Cast<EventSubscriber>();
        }

        internal IHandleQuery<TQuery, TResult> GetQueryHandler<TQuery, TResult>() 
            where TQuery : IQuery<TResult> => 
            Single<IHandleQuery<TQuery, TResult>>();

        internal void RegisterCommandHandler<TCommandHandler, TCommand>()
            where TCommandHandler : class, IHandleCommand<TCommand>
            where TCommand : ICommand => 
            RegisterTransient<IHandleCommand<TCommand>, TCommandHandler>();

        internal void RegisterQueryHandler<TQueryHandler, TQuery, TResult>()
            where TQueryHandler : class, IHandleQuery<TQuery, TResult>
            where TQuery : IQuery<TResult> =>
            RegisterTransient<IHandleQuery<TQuery, TResult>, TQueryHandler>();

        internal void RegisterStatefulEventHandler<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : class, IHandleEvent<TEvent>, IVolatileState
        {
            if (!IsRegistered<TEventHandler>())
                RegisterSingleton<TEventHandler, TEventHandler>();

            CopySingleton<TEventHandler, IHandleEvent<TEvent>>();
        }

        internal void RegisterStatelessEventHandler<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : class, IHandleEvent<TEvent> => 
            RegisterTransient<IHandleEvent<TEvent>, TEventHandler>();

        internal void RegisterVolatileState<T>()
            where T : class, IVolatileState
        {
            RegisterSingleton<T, T>();
            CopySingleton<T, IVolatileState>();
        }

        internal void ReleaseVolatileStates() => 
            ReleaseAll<IVolatileState>();

        private void InitializeCommandHandler(object iCommandHandler, ITenantContext tenant)
        {
            if (iCommandHandler is CommandHandler)
            {
                var commandHandler = (CommandHandler)iCommandHandler;
                commandHandler._session = _session;
                commandHandler.TenantId = tenant.TenantId;
            }
        }
    }
}