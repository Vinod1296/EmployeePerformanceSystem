namespace EmployeePerformance.Application.Exceptions
{
    public class CannotDeactivateLastActiveAdminException : Exception
    {
        public const string DefaultMessage = "Cannot deactivate the last active Admin.";

        public CannotDeactivateLastActiveAdminException()
            : base(DefaultMessage)
        {
        }

        public CannotDeactivateLastActiveAdminException(string? message)
            : base(string.IsNullOrWhiteSpace(message) ? DefaultMessage : message)
        {
        }
    }
}
