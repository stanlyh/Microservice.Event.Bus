using Microservice.Event.Bus.Test.Helpers;

namespace Microservice.Event.Bus.Test
{
    /// <summary>
    /// Pruebas unitarias a la clase <see cref="Subscription"/>
    /// </summary>
    public class SubscriptionTest
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
        /// Crea una nueva instancia de <see cref="SubscriptionTest"/>
        /// </summary>
        public SubscriptionTest() 
        {
            _userCreatedEvent = new UserCreatedEvent()
            {
                Id = new Random().Next(1, 100),
                Age = (ushort)new Random().Next(1, 100),
                Name = nameof(UserCreatedEvent.Name),
                User = nameof(UserCreatedEvent.User),
            };

            _userEventHandler = new UserEventHandler();
        }

        /// <summary>
        /// Valida que se inicie correctamente el estado del objeto
        /// </summary>
        [Fact]
        public void Create_InitializeStateObject_GetPropertiesValue()
        {
            // Arrange
            var eventType = _userEventHandler.GetType();
            var eventHandlerType = _userEventHandler.GetType();

            // Act
            var subscription = Subscription.Create<UserCreatedEvent, UserEventHandler>();

            // Assert
            Assert.Equal(eventType, subscription.EventType);
            Assert.Equal(eventHandlerType, subscription.EventHandlerType);
            Assert.Equal(eventType.Name, subscription.EventName);            
        }
    }
}
