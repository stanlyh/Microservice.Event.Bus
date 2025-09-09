using Microservice.Event.Bus.Abstractions;
using System.Collections.Concurrent;

namespace Microservice.Event.Bus.Internal.Queue
{
    /// <summary>
    /// Implementación por defecto para el servicio <see cref="IQueueService{TEventHandler, TEvent}"/>
    /// </summary>
    /// <typeparam name="TEventHandler">Manejador de eventos</typeparam>
    /// <typeparam name="TEvent">Evento de Integración</typeparam>
    public class QueueService<TEventHandler, TEvent> : IQueueService<TEventHandler, TEvent>
        where TEventHandler : IEventHandler<TEvent>
        where TEvent : EventBase
    {
        /// <summary>
        /// Contiene los eventos notificados por el Event Bus
        /// </summary>
        private readonly ConcurrentQueue<TEvent> _queueEvent = new ConcurrentQueue<TEvent>();

        /// <summary>
        /// Manejador de eventos
        /// </summary>
        private readonly TEventHandler _eventHandler;

        /// <summary>
        /// Crea una nueva instancia de <see cref="QueueService{TEvent}"/>
        /// </summary>
        /// <param name="eventHandler">Event Handler</param>
        public QueueService(TEventHandler eventHandler)
        {
            _eventHandler = eventHandler;
        }

        /// <summary>
        /// Agraga un objeto al final de la queue
        /// </summary>
        /// <param name="event">El objeto a agregar al final de la Queue</param>
        /// <exception cref="ArgumentNullException">Se genera cuando <paramref name="event"/>es nulo</exception>
        public void Enqueue(TEvent @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));

                var exist = _queueEvent.Any(e => e.Equals(@event));

                if (exist)
                    _queueEvent.Enqueue(@event);
            }
        }

        /// <summary>
        /// Tries to remove and return the object at the begining of the concurrent queue.
        /// </summary>
        /// <param name="token">Cancellation Token</param>
        /// <returns>Return Task that represents an asynchornous operation</returns>
        public async Task DequeueAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_queueEvent.TryDequeue(out TEvent @event))
                {
                    await _eventHandler.HandleAsync(@event, token);
                }
            }
        }
    }
}
