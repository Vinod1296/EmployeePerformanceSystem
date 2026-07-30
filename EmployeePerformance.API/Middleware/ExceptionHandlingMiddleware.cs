using EmployeePerformance.Application.Exceptions;

namespace EmployeePerformance.API.Middleware
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedException exception)
            {
                _logger.LogInformation(exception, "Unauthorized request.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (InvalidPageNumberException exception)
            {
                _logger.LogInformation(exception, "Invalid page number.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (InvalidPageSizeException exception)
            {
                _logger.LogInformation(exception, "Invalid page size.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (PageSizeExceededException exception)
            {
                _logger.LogInformation(exception, "Page size exceeded.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (InvalidSortByException exception)
            {
                _logger.LogInformation(exception, "Invalid sortBy requested.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (InvalidSortDirectionException exception)
            {
                _logger.LogInformation(exception, "Invalid sortDirection requested.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (UserNotFoundException exception)
            {
                _logger.LogInformation(exception, "User not found.");
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (InvalidRoleException exception)
            {
                _logger.LogInformation(exception, "Invalid role requested.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (SameRoleException exception)
            {
                _logger.LogInformation(exception, "Requested role already assigned.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (CannotChangeOwnRoleException exception)
            {
                _logger.LogInformation(exception, "Self role change blocked.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (CannotRemoveLastAdminException exception)
            {
                _logger.LogInformation(exception, "Last admin removal blocked.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (CannotDeactivateOwnAccountException exception)
            {
                _logger.LogInformation(exception, "Self deactivation blocked.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (CannotDeactivateLastActiveAdminException exception)
            {
                _logger.LogInformation(exception, "Last active admin deactivation blocked.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (UserStatusAlreadyUpdatedException exception)
            {
                _logger.LogInformation(exception, "Requested user status already assigned.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (AccountInactiveException exception)
            {
                _logger.LogInformation(exception, "Inactive account password change blocked.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (PasswordConfirmationMismatchException exception)
            {
                _logger.LogInformation(exception, "Password confirmation mismatch.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (PasswordStrengthException exception)
            {
                _logger.LogInformation(exception, "Password strength validation failed.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (CurrentPasswordIncorrectException exception)
            {
                _logger.LogInformation(exception, "Current password verification failed.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
            catch (NewPasswordMatchesCurrentPasswordException exception)
            {
                _logger.LogInformation(exception, "New password matched current password.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            }
        }
    }
}
