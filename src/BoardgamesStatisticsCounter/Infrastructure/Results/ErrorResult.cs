using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace BoardgamesStatisticsCounter.Infrastructure.Results
{
    public class ErrorResult
    {
        public ErrorResult(string code, string message)
        {
            ErrorCode = code;
            Message = message;
        }

        public ErrorResult(Exception? e)
        {
            ErrorCode = "internal_error";
            Message = "Something went wrong";
        }

        public string ErrorCode { get; set; }

        public string Message { get; set; }
    }
}
