namespace EmployeePerformance.Application.Exceptions
{
    public class InvalidRoleException : Exception
    {
        public const string DefaultMessage = "Invalid role.";

        public InvalidRoleException()
            : base(DefaultMessage)
        {
        }

        public InvalidRoleException(string? message)
            : base(string.IsNullOrWhiteSpace(message) ? DefaultMessage : message)
        {
        }
    }
}
