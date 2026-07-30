namespace EmployeePerformance.Application.Exceptions
{
    public class CannotDeactivateOwnAccountException : Exception
    {
        public const string DefaultMessage = "Cannot deactivate your own account.";

        public CannotDeactivateOwnAccountException()
            : base(DefaultMessage)
        {
        }

        public CannotDeactivateOwnAccountException(string? message)
            : base(string.IsNullOrWhiteSpace(message) ? DefaultMessage : message)
        {
        }
    }
}
