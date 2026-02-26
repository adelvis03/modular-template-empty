using System;
using System.Threading.Tasks;
using TemplateName.Shared.Abstractions.Messaging;

namespace TemplateName.Shared.Infrastructure.Messaging.Outbox;

public interface IOutbox
{
    bool Enabled { get; }
    Task SaveAsync(params IMessage[] messages);
    Task PublishUnsentAsync();
    Task CleanupAsync(DateTime? to = null);
}

