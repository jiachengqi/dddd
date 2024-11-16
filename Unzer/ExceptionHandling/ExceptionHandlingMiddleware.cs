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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            string errorMessage;
            string errorType = exception.GetType().Name;

            if (exception is ApplicationExceptionBase customException)
            {
                // Use status code from the custom exception
                statusCode = customException.StatusCode;
                errorMessage = customException.Message;
            }
            else
            {
                // Fallback for unhandled exceptions
                statusCode = (int)HttpStatusCode.InternalServerError;
                errorMessage = "An unexpected error occurred. Please try again later.";
                Console.WriteLine($"Unhandled exception: {exception.GetType().Name}");
            }

            // Set the response status code and content type
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            // Create a structured error response
            var errorResponse = new
            {
                StatusCode = statusCode,
                ErrorType = errorType,
                ErrorMessage = errorMessage,
                TraceId = context.TraceIdentifier
            };

            // Serialize the error response to JSON
            var errorJson = JsonSerializer.Serialize(errorResponse);

            // Log the structured error response for audit and monitoring
            Console.WriteLine($"Error Response: {errorJson}");

            return context.Response.WriteAsync(errorJson);
        }
    }
}
