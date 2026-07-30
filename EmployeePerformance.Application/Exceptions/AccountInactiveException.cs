namespace EmployeePerformance.Application.Exceptions
{
    public class AccountInactiveException : Exception
    {
        public const string DefaultMessage = "Your account is inactive.";

        public AccountInactiveException()
            : base(DefaultMessage)
        {
        }
    }
}
