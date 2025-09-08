using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Event.Bus.Abstractions
{
    public interface IEventBus
    {
        /// <summary>
        /// Metodo encargado de publicar un evento de integración
        /// </summary>
        /// <param name="event">Información del evento a publicar</param>
        /// <param name="token">Cacellation token</param>
        /// <returns>System.Threading.Tasks.Task que representa a la operación asincrónica</returns>
        Task PublichAsync(EventBase @event, CancellationToken token);
        /// <summary>
        /// Metodo encargado de publicar un evento de integración
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="event">nformación del evento a publicar</param>
        /// <param name="token">Cacellation token</param>
        /// <returns>System.Threading.Tasks.Task que representa a la operación asincrónica, la información determinada por la implementación de la</returns>
        Task<TResult> PublishAsync<TResult>(EventBase @event, CancellationToken token);
        /// <summary>
        /// Metodo encargado de publicar un evento de integración
        /// </summary>
        /// <typeparam name="TEvent">Evento de integración a escuchar</typeparam>
        /// <typeparam name="TEventHandler">Manehador de eventos de integración (CallBack)</typeparam>
        /// <returns>System.Threading.Tasks.Task que representa a la operación asincrónica</returns>
        Task SubscribeAsync<TEvent, TEventHandler>() 
            where TEvent : EventBase 
            where TEventHandler : IEventHandler<TEvent>;
        /// <summary>
        /// Metodo encargado de cancelar la subscripción de un evento
        /// </summary>
        /// <typeparam name="TEvent">Evento de integración a escuchar</typeparam>
        /// <typeparam name="TEventHandler">Manejador de eventos de integración (CallBack)</typeparam>
        void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler: IEventHandler<TEvent>;
    }
}
