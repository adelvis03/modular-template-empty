using System;
using TemplateName.Shared.Abstractions.Contexts;

namespace TemplateName.Shared.Abstractions.Messaging;

public interface IMessageContext
{
    public Guid MessageId { get; }
    public IContext Context { get; }
}



