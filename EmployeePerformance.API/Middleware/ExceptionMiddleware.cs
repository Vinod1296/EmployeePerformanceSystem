using System.Net;
using System.Text.Json;
using EmployeePerformance.API.Models;
using EmployeePerformance.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace EmployeePerformance.API.Middleware
{
    public sealed class ExceptionMiddleware
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (Exception exception)
            {
                var (statusCode, message) = MapException(exception);
                _logger.LogError(exception,
                    "Unhandled exception while processing {Method} {Path}. TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);
                await WriteResponseAsync(context, statusCode, message);
            }
        }

        private static (int statusCode, string message) MapException(Exception exception)
        {
            return exception switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
                UserNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
                UnauthorizedException => (StatusCodes.Status401Unauthorized, exception.Message),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, exception.Message),
                ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
                InvalidOperationException => (StatusCodes.Status400BadRequest, exception.Message),
                InvalidPageNumberException => (StatusCodes.Status400BadRequest, exception.Message),
                InvalidPageSizeException => (StatusCodes.Status400BadRequest, exception.Message),
                PageSizeExceededException => (StatusCodes.Status400BadRequest, exception.Message),
                InvalidSortByException => (StatusCodes.Status400BadRequest, exception.Message),
                InvalidSortDirectionException => (StatusCodes.Status400BadRequest, exception.Message),
                InvalidRoleException => (StatusCodes.Status400BadRequest, exception.Message),
                SameRoleException => (StatusCodes.Status400BadRequest, exception.Message),
                CannotChangeOwnRoleException => (StatusCodes.Status400BadRequest, exception.Message),
                CannotRemoveLastAdminException => (StatusCodes.Status400BadRequest, exception.Message),
                CannotDeactivateOwnAccountException => (StatusCodes.Status400BadRequest, exception.Message),
                CannotDeactivateLastActiveAdminException => (StatusCodes.Status400BadRequest, exception.Message),
                UserStatusAlreadyUpdatedException => (StatusCodes.Status400BadRequest, exception.Message),
                AccountInactiveException => (StatusCodes.Status400BadRequest, exception.Message),
                PasswordConfirmationMismatchException => (StatusCodes.Status400BadRequest, exception.Message),
                PasswordStrengthException => (StatusCodes.Status400BadRequest, exception.Message),
                CurrentPasswordIncorrectException => (StatusCodes.Status400BadRequest, exception.Message),
                NewPasswordMatchesCurrentPasswordException => (StatusCodes.Status400BadRequest, exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };
        }

        private static async Task WriteResponseAsync(HttpContext context, int statusCode, string message)
        {
            if (context.Response.HasStarted)
            {
                throw new InvalidOperationException("The response has already started.");
            }

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var payload = new ErrorResponse
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            await JsonSerializer.SerializeAsync(context.Response.Body, payload, JsonOptions, context.RequestAborted);
        }
    }
}
