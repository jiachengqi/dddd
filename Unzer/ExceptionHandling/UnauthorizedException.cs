namespace Unzer.ExceptionHandling
{
    [Serializable]
    public class UnauthorizedException : ApplicationExceptionBase
    {
        public UnauthorizedException(string message)
            : base(message, 401) { }

        public UnauthorizedException(string message, Exception innerException)
            : base(message, innerException, 401) { }
    }
}
