using System;
using System.Runtime.Serialization;

namespace Unzer.ExceptionHandling
{
    // Base custom exception class for the application
    [Serializable]
    public abstract class ApplicationExceptionBase : Exception
    {
        public int StatusCode { get; }

        protected ApplicationExceptionBase(string message, int statusCode = 500) : base(message)
        {
            StatusCode = statusCode;
        }

        protected ApplicationExceptionBase(string message, Exception innerException, int statusCode = 500)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }

        protected ApplicationExceptionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            StatusCode = (int)info.GetValue("StatusCode", typeof(int));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("StatusCode", StatusCode);
        }
    }

    [Serializable]
    public class DataAccessException : ApplicationExceptionBase
    {
        public DataAccessException(string message)
            : base(message, 500) { }

        public DataAccessException(string message, Exception innerException)
            : base(message, innerException, 500) { }
    }

    [Serializable]
    public class NotFoundException : ApplicationExceptionBase
    {
        public NotFoundException(string message)
            : base(message, 404) { }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException, 404) { }
    }

    [Serializable]
    public class BusinessException : ApplicationExceptionBase
    {
        public BusinessException(string message)
            : base(message, 400) { }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException, 400) { }
    }

    [Serializable]
    public class UnauthorizedException : ApplicationExceptionBase
    {
        public UnauthorizedException(string message)
            : base(message, 401) { }

        public UnauthorizedException(string message, Exception innerException)
            : base(message, innerException, 401) { }
    }

    [Serializable]
    public class ConflictException : ApplicationExceptionBase
    {
        public ConflictException(string message)
            : base(message, 409) { }

        public ConflictException(string message, Exception innerException)
            : base(message, innerException, 409) { }
    }

    [Serializable]
    public class ExternalServiceException : ApplicationExceptionBase
    {
        public ExternalServiceException(string message)
            : base(message, 502) { }

        public ExternalServiceException(string message, Exception innerException)
            : base(message, innerException, 502) { }
    }

    [Serializable]
    public class ValidationException : ApplicationExceptionBase
    {
        public ValidationException(string message)
            : base(message, 422) { }

        public ValidationException(string message, Exception innerException)
            : base(message, innerException, 422) { }
    }

    [Serializable]
    public class DatabaseException : ApplicationExceptionBase
    {
        public DatabaseException(string message)
            : base(message, 500) { }

        public DatabaseException(string message, Exception innerException)
            : base(message, innerException, 500) { }
    }

    // Exception for service layer errors
    [Serializable]
    public class ServiceException : ApplicationExceptionBase
    {
        public ServiceException(string message)
            : base(message, 500) { }

        public ServiceException(string message, Exception innerException)
            : base(message, innerException, 500) { }
    }

    // Exception for bad request errors (validation issues or client-side mistakes)
    [Serializable]
    public class BadRequestException : ApplicationExceptionBase
    {
        public BadRequestException(string message)
            : base(message, 400) { }

        public BadRequestException(string message, Exception innerException)
            : base(message, innerException, 400) { }
    }
}
