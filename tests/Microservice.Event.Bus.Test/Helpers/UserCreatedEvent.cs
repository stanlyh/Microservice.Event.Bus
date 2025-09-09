using Microservice.Event.Bus.Abstractions;

namespace Microservice.Event.Bus.Test.Helpers
{
    public class UserCreatedEvent : EventBase
    {
        public UserCreatedEvent()
        {
        }

        public UserCreatedEvent(Guid idEvent, DateTime eventDate) : base(idEvent, eventDate)
        {
        }

        public int Id { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
    }
}
