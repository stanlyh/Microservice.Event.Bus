using Microservice.Event.Bus.Internal.Queue;
using Microservice.Event.Bus.Test.Helpers;

namespace Microservice.Event.Bus.Test.Internal.EventBusBackgroundService
{
    /// <summary>
    /// Prueba unitarias a la clase <see cref="QueueService{TEventHandler, TEvent}"/>
    /// </summary>
    public class EventBusBackgroundServiceTest
    {
        /// <summary>
        /// Event Handler que procesa los eventos de tipo <see cref="UserCreatedEvent"/>
        /// </summary>
        private readonly UserEventHandler _userEventHandler;

        /// <summary>
        /// Evento de integración usado cuando es creado un usuario
        /// </summary>
        private readonly UserCreatedEvent _userCreatedEvent;

        /// <summary>
        /// Servicio que administra los eventos notificados por el event bus
        /// </summary>
        private readonly QueueService<UserEventHandler, UserCreatedEvent> _queueService;

        /// <summary>
        /// Crea una nueva instancia de <see cref="EventBusBackgroundServiceTest"/>
        /// </summary>
        public EventBusBackgroundServiceTest()
        {
            _userCreatedEvent = new UserCreatedEvent()
            {
                Id = new Random().Next(1, 1000),
                Age = (ushort)new Random().Next(1, 100),
                Name = nameof(UserCreatedEvent.Name),
                User = nameof(UserCreatedEvent.User),
            };

            _userEventHandler = new UserEventHandler();

            _queueService = new QueueService<UserEventHandler, UserCreatedEvent>(_userEventHandler);
        }

        /// <summary>
        /// Valida que se obtenga el vento
        /// </summary>
        [Fact]
        public void ExcuteAsync_DequeueEvents_QueueEmty() 
        { 
            // Arrangle
            _queueService.Enqueue(_userCreatedEvent);
            var backgroundService =  new ES.EventBusBackgroundService<QueueService<UserEventHandler, UserCreatedEvent>, UserEventHandler, UserCreatedEvent>();

            // Act
            backgroundService.StartAsync(CancellationToken.None).ConfigureAwait(false);

            Thread.Sleep(TimeSpan.FromSeconds(5));

            // Assert
            Assert.NotNull(UserEventHandler.Events.FirstOrDefault(x => x.Value == _userCreatedEvent).Value);
            Assert.False(_queueService.Any());
        }
    }
}
