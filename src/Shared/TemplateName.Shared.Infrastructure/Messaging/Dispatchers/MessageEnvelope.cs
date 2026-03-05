using TemplateName.Shared.Abstractions.Messaging;

namespace TemplateName.Shared.Infrastructure.Messaging.Dispatchers;

public record MessageEnvelope(IMessage Message, IMessageContext MessageContext);



