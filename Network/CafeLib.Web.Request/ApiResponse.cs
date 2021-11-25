using System;

namespace CafeLib.Web.Request
{
    public class ApiResponse
    {
        public bool IsSuccessful { get; }

        public Exception Exception { get; }

        public ApiResponse()
        {
            IsSuccessful = true;
        }

        public ApiResponse(Exception exception)
        {
            IsSuccessful = false;
            Exception = exception;
        }

        public TException GetException<TException>() where TException : Exception => (TException)Exception;
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Result { get; }

        public ApiResponse(T result)
        {
            Result = result;
        }

        public ApiResponse(Exception exception)
            : base(exception)
        {
        }
    }
}
