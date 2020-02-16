using System;
using System.Diagnostics.CodeAnalysis;

namespace BoardgamesStatisticsCounter.Infrastructure.Results
{
    public class OperationResult<T>
    {
        public OperationResult(T result) => Result = result;

        public OperationResult(ErrorResult? error) => Error = error;

        public OperationResult(Exception? e) => Error = new ErrorResult(e);

        public bool IsSuccesfull => Error == null;

        [MaybeNull]
        public T Result { get; } = default(T) !;

        public ErrorResult? Error { get; }

        public static implicit operator OperationResult<T>(T result) => new OperationResult<T>(result);

        public static implicit operator OperationResult<T>(Exception e) => new OperationResult<T>(e);

        public static implicit operator OperationResult<T>(ErrorResult error) => new OperationResult<T>(error);
    }
}