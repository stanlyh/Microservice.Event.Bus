using Microservice.Event.Bus.Abstractions;

namespace Microservice.Event.Bus.Internal.Queue
{
    /// <summary>
    /// Servicio que administra los eventos notificados por el event bus
    /// </summary>
    /// <typeparam name="TEventHandler">Evento de Integración</typeparam>
    /// <typeparam name="TEvent">Manejaor de eventos</typeparam>
    public interface IQueueService<TEventHandler, in TEvent>
        where TEventHandler : IEventHandler<TEvent>
        where TEvent : EventBase
    {
        /// <summary>
        /// Agraga un objeto al final de la queue
        /// </summary>
        /// <param name="event">El objeto a agregar al final de la Queue</param>
        /// <exception cref="ArgumentNullException">Se genera cuando <paramref name="event"/>es nulo</exception>
        void Enqueue(TEvent @event);

        /// <summary>
        /// Tries to remove and return the object at the begining of the concurrent queue.
        /// </summary>
        /// <param name="token">Cancellation Token</param>
        /// <returns>Return Task that represents an asynchornous operation</returns>
        Task DequeueAsync(CancellationToken token);
    }
}
