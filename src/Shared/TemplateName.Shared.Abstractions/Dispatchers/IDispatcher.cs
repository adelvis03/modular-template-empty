using System.Threading;
using System.Threading.Tasks;
using TemplateName.Shared.Abstractions.Commands;
using TemplateName.Shared.Abstractions.Events;
using TemplateName.Shared.Abstractions.Queries;

namespace TemplateName.Shared.Abstractions.Dispatchers;

public interface IDispatcher
{
    Task SendAsync<T>(T command, CancellationToken cancellationToken = default) where T : class, ICommand;
    Task<T> SendAsync<TCommand, T>(TCommand command, CancellationToken cancellationToken = default) where TCommand : class, ICommand<T>;
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class, IEvent;
    Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}



