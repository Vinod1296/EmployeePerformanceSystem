namespace EmployeePerformance.Application.Exceptions
{
    public class PageSizeExceededException : Exception
    {
        public PageSizeExceededException()
            : base("Maximum page size is 100.")
        {
        }
    }
}
