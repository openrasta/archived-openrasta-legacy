using System;

namespace OpenRasta.OperationModel.Interceptors
{
    public class InterceptorException : Exception
    {
        public InterceptorException(string message) : base(message)
        {
        }

        public InterceptorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}