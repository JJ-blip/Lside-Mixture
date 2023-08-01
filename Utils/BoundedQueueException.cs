namespace Lside_Mixture.Utils
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class BoundedQueueException : Exception
    {
        public BoundedQueueException()
        {
        }

        public BoundedQueueException(string message)
            : base(message)
        {
        }

        public BoundedQueueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected BoundedQueueException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}