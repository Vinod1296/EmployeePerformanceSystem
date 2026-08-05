namespace EmployeePerformance.Application.Exceptions
{
    public class EmployeeAlreadyRegisteredException : Exception
    {
        public EmployeeAlreadyRegisteredException()
            : base("A user account already exists for this employee.")
        {
        }

        public EmployeeAlreadyRegisteredException(string? message)
            : base(message)
        {
        }
    }
}
