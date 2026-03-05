using System;
using TemplateName.Shared.Abstractions.Exceptions;

namespace TemplateName.Shared.Infrastructure.Exceptions;

public interface IExceptionCompositionRoot
{
    ExceptionResponse Map(Exception exception);
}



