using System.Threading;
using System.Threading.Tasks;

namespace TemplateName.Shared.Abstractions.Commands;

public interface ICommandHandler<in TCommand> where TCommand : class, ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<in TCommand, T>
    where TCommand : class, ICommand<T>
{
    Task<T> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

