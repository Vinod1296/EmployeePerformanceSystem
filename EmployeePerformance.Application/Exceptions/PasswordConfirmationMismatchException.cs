namespace EmployeePerformance.Application.Exceptions
{
    public class PasswordConfirmationMismatchException : Exception
    {
        public const string DefaultMessage = "New password and confirmation password do not match.";

        public PasswordConfirmationMismatchException()
            : base(DefaultMessage)
        {
        }
    }
}
