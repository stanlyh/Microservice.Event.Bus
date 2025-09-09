using Microservice.Event.Bus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Event.Bus.Test.Helpers
{
    public class UserCreateEventHandler : IEventHandler<UserCreatedEvent>
    {
        public Task HandleAsync(UserCreatedEvent data, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
