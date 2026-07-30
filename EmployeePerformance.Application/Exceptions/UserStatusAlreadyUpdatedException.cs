namespace EmployeePerformance.Application.Exceptions
{
    public class UserStatusAlreadyUpdatedException : Exception
    {
        public const string DefaultMessage = "User status is already updated.";

        public UserStatusAlreadyUpdatedException()
            : base(DefaultMessage)
        {
        }

        public UserStatusAlreadyUpdatedException(string? message)
            : base(string.IsNullOrWhiteSpace(message) ? DefaultMessage : message)
        {
        }
    }
}
