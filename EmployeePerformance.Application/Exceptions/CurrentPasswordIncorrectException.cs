namespace EmployeePerformance.Application.Exceptions
{
    public class CurrentPasswordIncorrectException : Exception
    {
        public const string DefaultMessage = "Current password is incorrect.";

        public CurrentPasswordIncorrectException()
            : base(DefaultMessage)
        {
        }
    }
}
