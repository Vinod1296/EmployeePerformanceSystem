namespace EmployeePerformance.Application.Exceptions
{
    public class InvalidSortByException : Exception
    {
        public InvalidSortByException()
            : base("Invalid sortBy.")
        {
        }
    }
}
