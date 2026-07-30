namespace EmployeePerformance.Application.Exceptions
{
    public class InvalidSortDirectionException : Exception
    {
        public InvalidSortDirectionException()
            : base("Invalid sortDirection.")
        {
        }
    }
}
