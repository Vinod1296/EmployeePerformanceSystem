namespace EmployeePerformance.Application.Exceptions
{
    public class InvalidPageNumberException : Exception
    {
        public InvalidPageNumberException()
            : base("pageNumber must be greater than 0.")
        {
        }
    }
}
