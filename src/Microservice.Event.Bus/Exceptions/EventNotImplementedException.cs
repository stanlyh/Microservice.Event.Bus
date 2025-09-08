using System.Runtime.Serialization;

namespace Microservice.Event.Bus.Exceptions
{
    /// <summary>
    /// Se genera uando se quiere acceder a un evento que no etsa registrado
    /// </summary>
    [Serializable]
    public class EventNotImplementedException : Exception
    {
        /// <summary>
        /// Crea una nueva instacia de <see cref="EventNotImplementedException"
        /// </summary>
        public EventNotImplementedException() { }

        /// <summary>
        /// Crea una nueva instacia de <see cref="EventNotImplementedException"
        /// </summary>
        /// <param name="message">Mensaje del error</param>
        public EventNotImplementedException(string message) : base(message) { }

        /// <summary>
        /// Crea una nueva instacia de <see cref="EventNotImplementedException"
        /// </summary>
        /// <param name="message">Mensaje del error</param>
        /// <param name="innerException">Inner Exception</param>
        public EventNotImplementedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Without this constructos, deserialization will fail
        /// </summary>
        /// <param name="info">Serialization Info</param>
        /// <param name="context">Streaming Context</param>
        protected EventNotImplementedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
