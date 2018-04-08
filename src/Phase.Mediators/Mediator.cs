using Phase.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Phase.Providers;

namespace Phase.Mediators
{
    public class Mediator : IMediator
    {
        protected readonly HandlerFactory _handlerFactory;

        public Mediator(HandlerFactory factory)
        {
            _handlerFactory = factory;
        }

        public Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken token)
        {
            var invoker = (QueryInvoker<TResult>)Activator.CreateInstance(typeof(QueryInvoker<,>).MakeGenericType(query.GetType(), typeof(TResult)));
            return invoker.Invoke(query, _handlerFactory, token);
        }

        public Task<T> ExecuteAsync<T>(ICommand<T> command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var invoker = (CommandInvoker<T>)Activator.CreateInstance(typeof(CommandInvoker<,>).MakeGenericType(command.GetType(), typeof(T)));
            return invoker.Invoke(command, _handlerFactory);
        }

        public Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var invoker = (VoidCommandInvoker)Activator.CreateInstance(typeof(VoidCommandInvoker<>).MakeGenericType(command.GetType()));
            return invoker.Invoke(command, _handlerFactory);
        }

        public Task InitializeAsync(string serviceInstanceName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task AbortAsync()
        {
            return Task.CompletedTask;
        }
    }
}