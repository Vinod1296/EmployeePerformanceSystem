namespace EmployeePerformance.Application.Exceptions
{
    public class SameRoleException : Exception
    {
        public const string DefaultMessage = "User already has this role.";

        public SameRoleException()
            : base(DefaultMessage)
        {
        }

        public SameRoleException(string? message)
            : base(string.IsNullOrWhiteSpace(message) ? DefaultMessage : message)
        {
        }
    }
}
