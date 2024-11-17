
namespace Unzer.ExceptionHandling
{
    [Serializable]
    public class ServiceException : ApplicationExceptionBase
    {
        public ServiceException(string message)
            : base(message, 500) { }

        public ServiceException(string message, Exception innerException)
            : base(message, innerException, 500) { }
    }
}
