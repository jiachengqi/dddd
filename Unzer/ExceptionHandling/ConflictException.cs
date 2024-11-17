namespace Unzer.ExceptionHandling
{
    [Serializable]
    public class ConflictException : ApplicationExceptionBase
    {
        public ConflictException(string message)
            : base(message, 409) { }

        public ConflictException(string message, Exception innerException)
            : base(message, innerException, 409) { }
    }
}
