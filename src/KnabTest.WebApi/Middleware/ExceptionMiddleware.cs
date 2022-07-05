using System.Net;
using KnabTest.Logic.Exceptions;
using KnabTest.Logic.Models;

namespace KnabTest.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext).ConfigureAwait(false);
        }
        catch (BusinessException ex)
        {
            _logger.LogError(ex, Resources.ApplicationError);
            await HandleBusinessExceptionAsync(httpContext, ex).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, Resources.UnexpectedError);
            await HandleUnexpectedExceptionAsync(httpContext, ex).ConfigureAwait(false);
        }
    }
    
    private async Task HandleUnexpectedExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(new ErrorDetails
        {
            StatusCode = context.Response.StatusCode,
            Message = Resources.UnexpectedError 
        }.ToString()).ConfigureAwait(false);
    }
    
    private async Task HandleBusinessExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        await context.Response.WriteAsync(new ErrorDetails
        {
            StatusCode = context.Response.StatusCode,
            Message = Resources.ApplicationError + " : " + exception.Message
        }.ToString()).ConfigureAwait(false);
    }
}