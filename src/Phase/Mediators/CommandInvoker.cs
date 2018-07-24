using Phase.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Mediators
{
    internal abstract class VoidCommandInvoker
    {
        internal abstract Task InvokeAsync(ITenantContext tenant, ICommand command, DependencyResolver resolver, CancellationToken cancellationToken);
    }

    internal class VoidCommandInvoker<TCommand> : VoidCommandInvoker
        where TCommand : ICommand
    {
        internal override Task InvokeAsync(ITenantContext tenant, ICommand command, DependencyResolver resolver, CancellationToken cancellationToken) => 
            resolver.GetCommandHandler<TCommand>(tenant).Execute((TCommand)command, cancellationToken);
    }
}