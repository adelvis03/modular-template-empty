using System;

namespace TemplateName.Shared.Infrastructure.Exceptions.ExceptionClasses;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message){}
    public NotFoundException(string name, object key) : base($"Entity {name} with key {key} was not found."){}
}