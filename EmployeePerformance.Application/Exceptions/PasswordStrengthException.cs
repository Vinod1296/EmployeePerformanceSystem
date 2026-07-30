namespace EmployeePerformance.Application.Exceptions
{
    public class PasswordStrengthException : Exception
    {
        public const string DefaultMessage = "Password does not meet minimum strength requirements.";

        public PasswordStrengthException()
            : base(DefaultMessage)
        {
        }
    }
}
