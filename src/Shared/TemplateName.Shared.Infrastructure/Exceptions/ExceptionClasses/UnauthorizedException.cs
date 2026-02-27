using System;

namespace TemplateName.Shared.Infrastructure.Exceptions.ExceptionClasses;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message){}
    public UnauthorizedException(string name, object key) : base($"You are not authorized to access {name} with ID {key}."){}
}
