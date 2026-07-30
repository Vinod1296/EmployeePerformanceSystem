namespace EmployeePerformance.Application.Exceptions
{
    public class CannotChangeOwnRoleException : Exception
    {
        public const string DefaultMessage = "Cannot change your own role.";

        public CannotChangeOwnRoleException()
            : base(DefaultMessage)
        {
        }

        public CannotChangeOwnRoleException(string? message)
            : base(string.IsNullOrWhiteSpace(message) ? DefaultMessage : message)
        {
        }
    }
}
