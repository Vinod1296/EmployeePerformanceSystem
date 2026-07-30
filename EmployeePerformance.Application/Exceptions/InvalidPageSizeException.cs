namespace EmployeePerformance.Application.Exceptions
{
    public class InvalidPageSizeException : Exception
    {
        public InvalidPageSizeException()
            : base("pageSize must be greater than 0.")
        {
        }
    }
}
