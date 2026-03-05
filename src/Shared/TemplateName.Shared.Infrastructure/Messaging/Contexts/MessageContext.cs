using System;
using TemplateName.Shared.Abstractions.Contexts;
using TemplateName.Shared.Abstractions.Messaging;

namespace TemplateName.Shared.Infrastructure.Messaging.Contexts;

public class MessageContext : IMessageContext
{
    public Guid MessageId { get; }
    public IContext Context { get; }

    public MessageContext(Guid messageId, IContext context)
    {
        MessageId = messageId;
        Context = context;
    }
}



