using FCG.Users.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace FCG.Users.API.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, ex.Message);

            await WriteResponseAsync(
                context,
                HttpStatusCode.BadRequest,
                ex.Message);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, ex.Message);

            await WriteResponseAsync(
                context,
                HttpStatusCode.NotFound,
                ex.Message);
        }
        catch (UnauthorizedException ex)
        {
            _logger.LogWarning(ex, ex.Message);

            await WriteResponseAsync(
                context,
                HttpStatusCode.Unauthorized,
                ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            await WriteResponseAsync(
                context,
                HttpStatusCode.InternalServerError,
                "Erro interno do servidor.");
        }
    }

    private static async Task WriteResponseAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string message)
    {
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            message
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}