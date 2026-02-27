using System;

namespace TemplateName.Shared.Abstractions.Exceptions.ExceptionClasses;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException(string message, string details) : base(message)
    {
        Details = details;
    }

    public string Details { get; }
}