using System.Runtime.Serialization;

namespace Microservice.Event.Bus.Exceptions
{
    /// <summary>
    /// This exception that is thrown when ocurre un error interno en subscriptio manager
    /// </summary>
    [Serializable]
    public class EventBusException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the EventBusException class
        /// </summary>
        public EventBusException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the EventBusException class a specified error message
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public EventBusException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EventBusException class with a specified error
        /// message and a reference to the inner exception that is the cause of this execption.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothig in Visual Basic)</param>
        public EventBusException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Without this constructos, deserialization will fail
        /// </summary>
        /// <param name="info">Serialization Info</param>
        /// <param name="context">Streaming Context</param>
        protected EventBusException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
