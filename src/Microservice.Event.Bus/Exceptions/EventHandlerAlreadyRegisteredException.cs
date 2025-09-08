using Microservice.Event.Bus.Abstractions;

namespace Microservice.Event.Bus.Exceptions
{
    /// <summary>
    /// This exception that is thrown when an event handler already registered in the subscription manager
    /// </summary>
    [Serializable]
    public class EventHandlerAlreadyRegisteredException<TEvent, TEventHandler> : Exception
        where TEvent : EventBase
        where TEventHandler : IEventHandler<TEvent>
    {
        /// <summary>
        /// Gets the event type
        /// </summary>
        public Type EventType { get => typeof(TEvent); }

        /// <summary>
        /// Gets the event handler type
        /// </summary>
        public Type EventHandlerType { get => typeof(TEventHandler); }

        /// <summary>
        /// Initializes a new instance of the EventHandlerAlreadyRegisteredException class
        /// </summary>
        public EventHandlerAlreadyRegisteredException()
            : base($"Handler Type {typeof(TEventHandler).Name} already registered for '{typeof(TEvent)}'")
        { }

        /// <summary>
        /// Initializes a new instance of the EventHandlerAlreadyRegisteredException class with a specified error message. 
        /// </summary>
        //public EventHandlerAlreadyRegisteredException() { }
    }

}
