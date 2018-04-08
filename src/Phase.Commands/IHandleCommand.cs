using System.Threading.Tasks;

namespace Phase.Commands
{
    public interface IHandleCommand<in TCommand, TReturn>
        where TCommand : ICommand<TReturn>
    {
        Task<TReturn> Execute(TCommand command);
    }

    public interface IHandleCommand<in TCommand>
        where TCommand : ICommand
    {
        Task Execute(TCommand command);
    }
}