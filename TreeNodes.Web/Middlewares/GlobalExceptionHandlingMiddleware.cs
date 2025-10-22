using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TreeNodes.Application.Common.Exceptions;
using TreeNodes.Application.ExceptionJournal.Commands;
using TreeNodes.Auth.Exceptions;

namespace TreeNodes.Web.Middlewares;

/// <summary>
/// Middleware that captures all exceptions, writes to the journal, and returns specified JSON format.
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IMediator mediator)
    {
        // Enable buffering BEFORE processing the request so we can read the body later if needed
        context.Request.EnableBuffering();
        
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            string body = string.Empty;
            if (context.Request.ContentLength > 0)
            {
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                context.Request.Body.Position = 0;
                body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            var eventId = await mediator.Send(new WriteExceptionToJournalCommand(
                context.Request.Path,
                context.Request.Method,
                context.Request.QueryString.HasValue ? context.Request.QueryString.Value! : string.Empty,
                body,
                ex.GetType().Name,
                ex.Message,
                ex.StackTrace ?? string.Empty
            ));

            _logger.LogError(ex, "Unhandled exception captured. EventId={EventId}", eventId);

            context.Response.ContentType = "application/json";

            // Handle authentication exceptions with 401 Unauthorized
            if (ex is AuthenticationException || ex is InvalidTokenException || ex is InvalidPartnerCodeException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new 
                { 
                    type = "Unauthorized", 
                    id = eventId.ToString(), 
                    data = new { message = ex.Message } 
                });
            }
            else if (ex is Application.Common.Exceptions.NotFoundException)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { type = "Secure", id = eventId.ToString(), data = new { message = ex.Message } });
            }
            else if (ex is SecureException)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { type = "Secure", id = eventId.ToString(), data = new { message = ex.Message } });
            }
            else if (ex is ValidationException vex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { type = "Secure", id = eventId.ToString(), data = new { message = string.Join("; ", vex.Errors.Select(e => e.ErrorMessage)) } });
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { type = "Exception", id = eventId.ToString(), data = new { message = $"Internal server error ID = {eventId}" } });
            }
        }
    }
}


