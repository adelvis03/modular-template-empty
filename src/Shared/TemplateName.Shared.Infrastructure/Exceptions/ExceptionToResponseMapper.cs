using System;
using System.Collections.Concurrent;
using System.Net;
using Humanizer;
using TemplateName.Shared.Abstractions.Exceptions;
using TemplateName.Shared.Infrastructure.Exceptions.ExceptionClasses;

namespace TemplateName.Shared.Infrastructure.Exceptions;

public class ExceptionToResponseMapper : IExceptionToResponseMapper
{
    private static readonly ConcurrentDictionary<Type, string> Codes = new();

    public ExceptionResponse Map(Exception exception)
        => exception switch
        {
            NotFoundException ex => CreateResponse(ex, HttpStatusCode.NotFound),
            ConflictException ex => CreateResponse(ex, HttpStatusCode.Conflict),
            UnauthorizedException ex => CreateResponse(ex, HttpStatusCode.Unauthorized),
            BadRequestException ex => CreateResponse(ex, HttpStatusCode.BadRequest),
            UnauthorizedAccessException ex => CreateResponse(ex, HttpStatusCode.Forbidden),
            CustomException ex => CreateResponse(ex, HttpStatusCode.BadRequest),
            _ => CreateResponse(exception, HttpStatusCode.InternalServerError, "There was an error.")
        };

    private static ExceptionResponse CreateResponse(Exception ex, HttpStatusCode status, string message = null)
    {
        var error = new Error(GetErrorCode(ex), message ?? ex.Message);
        return new ExceptionResponse(new ErrorsResponse(error), status);
    }

    private record Error(string Code, string Message);

    private record ErrorsResponse(params Error[] Errors);

    private static string GetErrorCode(object exception)
    {
        var type = exception.GetType();
        return Codes.GetOrAdd(type, type.Name.Underscore().Replace("_exception", string.Empty));
    }
}