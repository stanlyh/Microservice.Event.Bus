using Microservice.Event.Bus.Internal.Queue;
using Microservice.Event.Bus.Test.Helpers;

namespace Microservice.Event.Bus.Test.Internal.Queue
{
    /// <summary>
    /// Pruebas unitarias a la clase <see cref="QueueSrvice{TEventHandler, TEvent}"/>
    /// </summary>
    public class QueueServiceTest
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
        /// Crea una nueva instancia de <see cref="QueueServiceTest"/>
        /// </summary>
        public QueueServiceTest() 
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
        /// Valida que se genere la excepción cuando el argumento es nulo
        /// </summary>
        [Fact]
        public void Enqueue_AddEventQueue_argumentNullException() 
        {
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => _queueService.Enqueue(null));

            // Assert
            Assert.NotEmpty(exception.Message);
        }

        /// <summary>
        /// Valida que la queue sea igual cuando el evento ya ha sido agregado
        /// </summary>
        [Fact]
        public void Enqueue_EventAlreadyExist_TailRemainsTheSame()
        {
            // Arrange
            _queueService.Enqueue(_userCreatedEvent);

            // Act
            _queueService.Enqueue(_userCreatedEvent);

            // Assert
            Assert.Equal(1, _queueService.Count);
        }

        /// <summary>
        /// Valida que se registre el evento en la Queue
        /// </summary>
        [Fact]
        public void Enwqueue_Add_QueueNotEmpty()
        {
            // Act
            _queueService.Enqueue(_userCreatedEvent);

            // Assert
            Assert.True(_queueService.Any());
        }

        /// <summary>
        /// Valida que se obtenga el evento
        /// </summary>
        [Fact]
        public void DequeueAsync_Get_QueueEmpty()
        {
            // Arrange
            _queueService.Enqueue(_userCreatedEvent);

            // Act
            Task.Run(() => _queueService.DequeueAsync(CancellationToken.None));

            Thread.Sleep(TimeSpan.FromSeconds(5));

            // Assert
            Assert.NotNull(UserEventHandler.Events.FirstOrDefault(x => x.Value == _userCreatedEvent).Value);
            Assert.False(_queueService.Any());
        }
    }
}
