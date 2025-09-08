namespace Microservice.Event.Bus.Abstractions
{
    /// <summary>
    /// Interfaz base para implementar un manejador de ebventos a partir de un evento definido
    /// </summary>
    /// <typeparam name="TEvent">Evento de Integración</typeparam>
    public interface IEventHandler<in TEvent>
        where TEvent : EventBase
    {
        /// <summary>
        /// Invocado por el event us cuando detecta un evento al que se esta subdcrito
        /// </summary>
        /// <param name="data">Información del evento</param>
        /// <param name="token">Token de cancelación</param>
        /// <returns>System,Threading.Tasks.Task que representa la operación asincrónica</returns>
        Task HandleAsync(TEvent data, CancellationToken token);
    }
}
