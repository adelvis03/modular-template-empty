using System.Threading;
using System.Threading.Tasks;
using TemplateName.Shared.Abstractions.Messaging;

namespace TemplateName.Shared.Infrastructure.Messaging.Dispatchers;

public interface IAsyncMessageDispatcher
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, IMessage;
}

