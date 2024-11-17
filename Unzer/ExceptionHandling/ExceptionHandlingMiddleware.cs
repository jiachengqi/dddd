using System;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Unzer.ExceptionHandling;

namespace Unzer.ExceptionHandling
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApplicationExceptionBase ex)
            {
                _logger.LogError(ex, "An application exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex.StatusCode, ex.Message, ex.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.", "InternalServerError");
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, int statusCode, string errorMessage, string errorType)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                StatusCode = statusCode,
                ErrorType = errorType,
                ErrorMessage = errorMessage,
                TraceId = context.TraceIdentifier
            };

            var errorJson = JsonSerializer.Serialize(errorResponse);

            return context.Response.WriteAsync(errorJson);
        }
    }
}
