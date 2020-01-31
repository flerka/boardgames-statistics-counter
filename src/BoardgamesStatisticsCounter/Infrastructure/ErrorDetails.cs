namespace BoardgamesStatisticsCounter.Infrastructure
{
    public class ErrorDetails
    {
        public ErrorDetails(int code, string message)
        {
            StatusCode = code;
            Message = message;
        }

        public int StatusCode { get; set; }

        public string Message { get; set; }
    }
}
