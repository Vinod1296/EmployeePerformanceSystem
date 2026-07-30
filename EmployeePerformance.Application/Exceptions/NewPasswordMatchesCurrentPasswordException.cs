namespace EmployeePerformance.Application.Exceptions
{
    public class NewPasswordMatchesCurrentPasswordException : Exception
    {
        public const string DefaultMessage = "New password cannot be the same as the current password.";

        public NewPasswordMatchesCurrentPasswordException()
            : base(DefaultMessage)
        {
        }
    }
}
