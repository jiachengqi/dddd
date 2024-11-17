using System;
using System.Runtime.Serialization;

namespace Unzer.ExceptionHandling
{
    [Serializable]
    public abstract class ApplicationExceptionBase : Exception
    {
        public int StatusCode { get; }

        protected ApplicationExceptionBase(string message, int statusCode = 500)
            : base(message)
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
            info.AddValue("StatusCode", StatusCode);
            base.GetObjectData(info, context);
        }
    }
}
