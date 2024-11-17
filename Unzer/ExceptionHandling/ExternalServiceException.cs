using System;

namespace Unzer.ExceptionHandling
{
    [Serializable]
    public class ExternalServiceException : ApplicationExceptionBase
    {
        public ExternalServiceException(string message)
            : base(message, 502) { }

        public ExternalServiceException(string message, Exception innerException)
            : base(message, innerException, 502) { }
    }
}
