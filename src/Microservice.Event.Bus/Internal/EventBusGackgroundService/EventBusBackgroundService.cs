using Microservice.Event.Bus.Abstractions;
using Microservice.Event.Bus.Internal.Queue;
using Microsoft.Extensions.Hosting;

namespace Microservice.Event.Bus.Internal.EventBusGackgroundService
{
    /// <summary>
    /// Procesa las queue que se registraron al inicar la aplicación
    /// </summary>
    /// <typeparam name="TQueueService">Queue Service que contiene los eventos notificados por el Event Bus</typeparam>
    /// <typeparam name="TEventHandler">Manejador de eventos</typeparam>
    /// <typeparam name="TEvent">Evento de Integración</typeparam>
    public class EventBusBackgroundService<TQueueService, TEventHandler, TEvent> : BackgroundService, IEventBusBackgroundService<TQueueService, TEventHandler, TEvent>
        where TQueueService : IQueueService<TEventHandler, TEvent>
        where TEventHandler : IEventHandler<TEvent>
        where TEvent : EventBase
    {
        /// <summary>
        /// Servicio que administra los eventos notificados por el event bus
        /// </summary>
        private readonly TQueueService _queueService;

        /// <summary>
        /// Crea una nueva instancia de <see cref="EventBusBackgroundService{TQueueService, TEvent}"/>
        /// </summary>
        /// <param name="queueService">Servicio que administra los eventos notificados por el event bus</param>
        public EventBusBackgroundService(TQueueService queueService) 
        {
            _queueService = queueService;
        }

        /// <summary>
        /// This method is called when the Microsoft.Extensions.Hosting.IHostedService stars.
        /// The implementation should return a task that represents rhe lifetime of the long runnin operation(s) beging performed.
        /// </summary>
        /// <param name="stoppingToken">Trigger when Microsoft.Extensions.Hosting.IHostedService.Stoped(System.Threading.CancellationToken) is called</param>
        /// <returns>Task that represents the long running operations.</returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => _queueService.DequeueAsync(stoppingToken), stoppingToken);
        }
    }

}
