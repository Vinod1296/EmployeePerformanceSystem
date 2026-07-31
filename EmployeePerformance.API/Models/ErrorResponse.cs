namespace EmployeePerformance.API.Models
{
    public sealed class ErrorResponse
    {
        public bool Success { get; init; }

        public int StatusCode { get; init; }

        public string Message { get; init; } = string.Empty;

        public DateTime Timestamp { get; init; }
    }
}
