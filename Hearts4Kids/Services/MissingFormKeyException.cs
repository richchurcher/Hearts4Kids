using System;
using System.Runtime.Serialization;

namespace Hearts4Kids.Services
{
    [Serializable]
    internal class MissingFormKeyException : Exception
    {
        public MissingFormKeyException()
        {
        }

        public MissingFormKeyException(string keyName) : base(keyName + " missing from form data")
        {
        }

        public MissingFormKeyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingFormKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}