using Microservice.Event.Bus.Abstractions;
using Microservice.Event.Bus.Internal.Queue;

namespace Microservice.Event.Bus.Internal.EventBusGackgroundService
{
    /// <summary>
    /// Procesa las queue que se registraron al iniciar la aplicación
    /// </summary>
    /// <typeparam name="TQueueService">Queue Service que contiene los eventos notificados por el Event Bus</typeparam>
    /// <typeparam name="TEventHandler">Manejador de eventos</typeparam>
    /// <typeparam name="TEvent">Evento de iNtegración</typeparam>
    public interface IEventBusBackgroundService<TQueueService, TEventHandler, TEvent>
        where TQueueService : IQueueService<TEventHandler, TEvent>
        where TEventHandler : IEventHandler<TEvent>
        where TEvent : EventBase
    {
    }
}
