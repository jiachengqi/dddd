
namespace Unzer.ExceptionHandling
{
    [Serializable]
    public class NotFoundException : ApplicationExceptionBase
    {
        public NotFoundException(string message)
            : base(message, 404) { }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException, 404) { }
    }
}
