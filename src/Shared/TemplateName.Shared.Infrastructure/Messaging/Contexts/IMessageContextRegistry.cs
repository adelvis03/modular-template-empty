using TemplateName.Shared.Abstractions.Messaging;

namespace TemplateName.Shared.Infrastructure.Messaging.Contexts;

public interface IMessageContextRegistry
{
    void Set(IMessage message, IMessageContext context);
}



