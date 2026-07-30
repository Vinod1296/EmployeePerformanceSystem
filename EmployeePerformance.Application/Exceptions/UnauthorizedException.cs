namespace EmployeePerformance.Application.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public const string DefaultMessage = "Invalid username or password";

        public UnauthorizedException()
            : base(DefaultMessage)
        {
        }

        public UnauthorizedException(string? message)
            : base(string.IsNullOrWhiteSpace(message) ? DefaultMessage : message)
        {
        }
    }
}
