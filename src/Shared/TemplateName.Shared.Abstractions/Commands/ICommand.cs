using TemplateName.Shared.Abstractions.Messaging;

namespace TemplateName.Shared.Abstractions.Commands;

//Marker
public interface ICommand : IMessage
{
}

public interface ICommand<T> : IMessage
{
}

