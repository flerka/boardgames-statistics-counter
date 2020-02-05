namespace BoardgamesStatisticsCounter.Infrastructure.Results
{
    public class InvalidMessageFormatErrorResult : ErrorResult
    {
        private const string Code = "invalid_message";
        private const string Text = "Can't parse message format";

        public InvalidMessageFormatErrorResult() 
            : base(Code, Text)
        {
        }
    }
}
