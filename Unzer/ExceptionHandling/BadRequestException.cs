namespace Unzer.ExceptionHandling
{
    [Serializable]
    public class BadRequestException : ApplicationExceptionBase
    {
        public BadRequestException(string message)
            : base(message, 400) { }

        public BadRequestException(string message, Exception innerException)
            : base(message, innerException, 400) { }
    }
}
