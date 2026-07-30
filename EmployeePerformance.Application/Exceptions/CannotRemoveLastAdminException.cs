namespace EmployeePerformance.Application.Exceptions
{
    public class CannotRemoveLastAdminException : Exception
    {
        public const string DefaultMessage = "Cannot remove the last remaining Admin.";

        public CannotRemoveLastAdminException()
            : base(DefaultMessage)
        {
        }

        public CannotRemoveLastAdminException(string? message)
            : base(string.IsNullOrWhiteSpace(message) ? DefaultMessage : message)
        {
        }
    }
}
