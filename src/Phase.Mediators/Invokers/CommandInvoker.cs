using Phase.Commands;
using System.Threading.Tasks;

namespace Phase.Mediators
{
    public abstract class VoidCommandInvoker
    {
        public abstract Task Invoke(ICommand command, HandlerFactory factory);

        protected TCommandHandler GetCommandHandler<TCommandHandler>(HandlerFactory factory)
        {
            return (TCommandHandler)factory(typeof(TCommandHandler));
        }
    }

    public class VoidCommandInvoker<TCommand> : VoidCommandInvoker
        where TCommand : ICommand
    {
        public override Task Invoke(ICommand command, HandlerFactory factory)
        {
            return GetCommandHandler<IHandleCommand<TCommand>>(factory)
                .Execute((TCommand)command);
        }
    }

    public abstract class CommandInvoker<T>
    {
        public abstract Task<T> Invoke(ICommand<T> command, HandlerFactory factory);

        protected TCommandHandler GetCommandHandler<TCommandHandler>(HandlerFactory factory)
        {
            return (TCommandHandler)factory(typeof(TCommandHandler));
        }
    }

    public class CommandInvoker<TCommand, TReturn> : CommandInvoker<TReturn>
        where TCommand : ICommand<TReturn>
    {
        public override Task<TReturn> Invoke(ICommand<TReturn> command, HandlerFactory factory)
        {
            return GetCommandHandler<IHandleCommand<TCommand, TReturn>>(factory)
                .Execute((TCommand)command);
        }
    }
}