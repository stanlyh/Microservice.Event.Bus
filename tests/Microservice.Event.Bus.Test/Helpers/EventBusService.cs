using Microservice.Event.Bus.Abstractions;

namespace Microservice.Event.Bus.Test.Helpers
{
    /// <summary>
    /// Implementación del Bus de eventos
    /// </summary>
    public class EventBusService : IEventBus
    {
        /// <summary>
        /// Subscription Manager
        /// </summary>
        public ISubscriptionManager Subscription { get; set; }

        /// <summary>
        /// Crea una instancia de <see cref="EventBusService"/>
        /// </summary>
        /// <param name="subscriptionManager"></param>
        public EventBusService(ISubscriptionManager subscriptionManager) 
        {
            this.Subscription = subscriptionManager;
        }

        /// <summary>
        /// Metodo encargado de publicar un evento de integración
        /// </summary>
        /// <param name="event">Informacion del evento a publicar</param>
        /// <param name="token">Cancellation Token</param>
        /// <returns>Task que representa la operacion asincronica</returns>
        public Task PublishAsync(EventBase @event, CancellationToken token)
        {
            return Task.CompletedTask;
        }


    }
}
