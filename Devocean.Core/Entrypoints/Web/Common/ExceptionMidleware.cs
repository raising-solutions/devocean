using Devocean.Core.Application.UseCases.Common;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Devocean.Core.Entrypoints.Web.Common;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            await HandleExceptionAsync(context, e);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var statusCode = GetStatusCode(exception);

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = statusCode;
        
        switch (statusCode)
        {
            case StatusCodes.Status404NotFound:
                await httpContext.Response.WriteAsJsonAsync(new 
                { 
                    title = GetTitle(exception),
                    status = statusCode,
                    detail = exception.Message
                });
                break;
            case StatusCodes.Status422UnprocessableEntity:
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    title = GetTitle(exception),
                    status = statusCode,
                    detail = exception.Message,
                    errors = GetErrors(exception)
                });
                break;
            default:
                await httpContext.Response.WriteAsJsonAsync(exception switch
                {
                    ServiceException serviceException => new
                    {
                        title = serviceException.Title,
                        status = serviceException.Status,
                        detail = serviceException.Detail,
                        errors = serviceException.Errors
                    },
                    _ => new
                    {
                        title = "Ocorreu um erro na operação",
                        status = statusCode,
                        detail = "Tente novamente ou entre em contato com o nosso atendimento",
                        errors = new List<string>()
                    }
                });
                break;
        }
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            InvalidOperationException => StatusCodes.Status400BadRequest,
            ArgumentNullException => StatusCodes.Status400BadRequest,
            ResourceNotFoundException => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status422UnprocessableEntity,
            ServiceException serviceException => serviceException.Status,
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GetTitle(Exception exception) =>
        exception switch
        {
            ResourceNotFoundException => "Resource not found",
            ValidationException applicationException => "Validation Error",
            _ => "Server Error"
        };

    private static IEnumerable<ValidationFailure>? GetErrors(Exception exception)
    {
        if (exception is ValidationException validationException)
        {
            return validationException.Errors;
        }

        return null;
    }
}