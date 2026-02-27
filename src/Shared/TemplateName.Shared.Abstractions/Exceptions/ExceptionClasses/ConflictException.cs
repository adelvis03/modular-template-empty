using System;

namespace TemplateName.Shared.Abstractions.Exceptions.ExceptionClasses;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message){}
    public ConflictException(string name, object key) : base($"Entity {name} with key {key} conflicts with another entity."){}
}