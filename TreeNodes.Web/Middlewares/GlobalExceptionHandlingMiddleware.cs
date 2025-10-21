using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TreeNodes.Application.Common.Exceptions;
using TreeNodes.Application.ExceptionJournal.Commands;

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
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Request.EnableBuffering();
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
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            if (ex is SecureException)
            {
                await context.Response.WriteAsJsonAsync(new { type = "Secure", id = eventId.ToString(), data = new { message = ex.Message } });
            }
            else if (ex is ValidationException vex)
            {
                await context.Response.WriteAsJsonAsync(new { type = "Secure", id = eventId.ToString(), data = new { message = string.Join("; ", vex.Errors.Select(e => e.ErrorMessage)) } });
            }
            else
            {
                await context.Response.WriteAsJsonAsync(new { type = "Exception", id = eventId.ToString(), data = new { message = $"Internal server error ID = {eventId}" } });
            }
        }
    }
}


