namespace EmployeePerformance.Application.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public const string DefaultMessage = "User not found.";

        public UserNotFoundException()
            : base(DefaultMessage)
        {
        }

        public UserNotFoundException(string? message)
            : base(string.IsNullOrWhiteSpace(message) ? DefaultMessage : message)
        {
        }
    }
}
