# Microservice.Event.Bus

Biblioteca de bus de eventos para microservicios en .NET 8 (C#). Proporciona un sistema ligero para publicar eventos y enrutar esos eventos a handlers suscritos, soportando entrega síncrona y asíncrona mediante un servicio background y una cola en memoria.

---

## Arquitectura general

```mermaid
graph TD
    App["Aplicación / Microservicio"]
    IEB["IEventBus"]
    SM["SubscriptionManager"]
    QS["QueueService&lt;THandler, TEvent&gt;"]
    BG["EventBusBackgroundService"]
    EH["IEventHandler&lt;TEvent&gt;"]

    App -->|"PublishAsync / SubscribeAsync"| IEB
    IEB -->|"Registra / consulta suscripciones"| SM
    IEB -->|"Encola evento"| QS
    BG -->|"DequeueAsync (loop)"| QS
    QS -->|"HandleAsync"| EH
```

---

## Flujo de publicación de un evento

```mermaid
sequenceDiagram
    participant App as Aplicación
    participant Bus as IEventBus
    participant SM as SubscriptionManager
    participant QS as QueueService
    participant BG as BackgroundService
    participant EH as IEventHandler

    App->>Bus: PublishAsync(event)
    Bus->>SM: GetHandlers<TEvent>()
    SM-->>Bus: List<Subscription>
    Bus->>QS: Enqueue(event)
    Note over BG: Ejecuta en segundo plano
    loop Mientras no se cancele
        BG->>QS: DequeueAsync(token)
        QS->>EH: HandleAsync(event, token)
    end
```

---

## Flujo de suscripción y cancelación

```mermaid
sequenceDiagram
    participant App as Aplicación
    participant Bus as IEventBus
    participant SM as SubscriptionManager

    App->>Bus: SubscribeAsync<TEvent, THandler>()
    Bus->>SM: AddSubscription<TEvent, THandler>()
    SM-->>Bus: OK (o EventHandlerAlreadyRegisteredException)

    App->>Bus: Unsubscribe<TEvent, THandler>()
    Bus->>SM: RemoveSubscription<TEvent, THandler>()
    SM-->>SM: Dispara OnEventRemoved si lista queda vacía
```

---

## Modelo de clases

```mermaid
classDiagram
    class EventBase {
        +Guid IdEvent
        +DateTime EventDate
        +Equals(EventBase) bool
    }

    class IEventHandler~TEvent~ {
        <<interface>>
        +HandleAsync(TEvent, CancellationToken) Task
    }

    class IEventBus {
        <<interface>>
        +PublichAsync(EventBase, CancellationToken) Task
        +PublishAsync~TResult~(EventBase, CancellationToken) Task~TResult~
        +SubscribeAsync~TEvent, THandler~() Task
        +Unsubscribe~TEvent, THandler~() void
    }

    class Subscription {
        +string EventName
        +Type EventType
        +Type EventHandlerType
        +Create~TEvent, THandler~() Subscription$
    }

    class ISubscriptionManager {
        <<interface>>
        +AddSubscription~TEvent, THandler~() void
        +RemoveSubscription~TEvent, THandler~() void
        +HasSubscriptionsForEvent~TEvent~() bool
        +GetHandlers~TEvent~() IEnumerable~Subscription~
        +FindSubscription~TEvent, THandler~() Subscription
        +FindSubscriptions~TEvent~() List~Subscription~
        +Clear() void
        +Any() bool
        +OnEventRemoved EventHandler~Subscription~
    }

    class SubscriptionManager {
        -Dictionary handlers
    }

    class IQueueService~THandler, TEvent~ {
        <<interface>>
        +int Count
        +Any() bool
        +Enqueue(TEvent) void
        +DequeueAsync(CancellationToken) Task
    }

    class QueueService~THandler, TEvent~ {
        -ConcurrentQueue _queueEvent
        -THandler _eventHandler
    }

    class EventBusBackgroundService~TQueue, THandler, TEvent~ {
        -TQueue _queueService
        #ExecuteAsync(CancellationToken) Task
    }

    EventBase <|-- UserCreatedEvent : hereda
    IEventHandler~TEvent~ <|.. UserCreatedHandler : implementa
    ISubscriptionManager <|.. SubscriptionManager : implementa
    IQueueService~THandler,TEvent~ <|.. QueueService~THandler,TEvent~ : implementa
    SubscriptionManager "1" o-- "n" Subscription : contiene
    Subscription --> EventBase : referencia tipo
    Subscription --> IEventHandler~TEvent~ : referencia tipo
    QueueService~THandler,TEvent~ --> IEventHandler~TEvent~ : invoca
    EventBusBackgroundService~TQueue,THandler,TEvent~ --> IQueueService~THandler,TEvent~ : consume
```

---

## Estructura del proyecto

```
Microservice.Event.Bus/
├── src/
│   └── Microservice.Event.Bus/
│       ├── Abstractions/
│       │   ├── EventBase.cs            # Clase base para todos los eventos
│       │   ├── IEventBus.cs            # Interfaz principal del bus
│       │   └── IEventHandler.cs        # Interfaz de los handlers
│       ├── Exceptions/
│       │   ├── EventBusException.cs
│       │   ├── EventHandlerAlreadyRegisteredException.cs
│       │   ├── EventIsNotRegisteredException.cs
│       │   ├── EventNotExistException.cs
│       │   └── EventNotImplementedException.cs
│       ├── Internal/
│       │   ├── Queue/
│       │   │   ├── IQueueService.cs
│       │   │   └── QueueService.cs     # Cola en memoria thread-safe
│       │   └── EventBusGackgroundService/
│       │       ├── IEventBusBackgroundService.cs
│       │       └── EventBusBackgroundService.cs  # BackgroundService de .NET
│       ├── Extension/
│       │   └── EventBusExtensions.cs   # Métodos de extensión para DI
│       ├── ISubscriptionManager.cs
│       ├── SubscriptionManager.cs      # Registro en memoria de suscripciones
│       └── Subscription.cs             # Metadatos de una suscripción
└── tests/
    └── Microservice.Event.Bus.Test/
```

---

## Registro en el contenedor DI

El método de extensión `AddEventBus` escanea los ensamblados cargados buscando la implementación concreta de `IEventBus` y la registra automáticamente. `AddEventsHandlers<TStartup>` descubre todos los `IEventHandler<TEvent>` del ensamblado de inicio y registra por cada uno:

- Un `QueueService<THandler, TEvent>` (Singleton)
- Un `EventBusBackgroundService` (Hosted Service)
- El propio handler (Transient)

```mermaid
flowchart LR
    A["services.AddEventBus()"] --> B["Registra ISubscriptionManager → SubscriptionManager"]
    A --> C["Escanea ensamblados → encuentra impl. IEventBus\nla registra como Singleton"]

    D["services.AddEventsHandlers&lt;TStartup&gt;()"] --> E["Por cada IEventHandler&lt;TEvent&gt; encontrado:"]
    E --> F["Singleton: IQueueService&lt;THandler,TEvent&gt;"]
    E --> G["HostedService: EventBusBackgroundService"]
    E --> H["Transient: THandler"]

    I["provider.SubscribeEventsHandlers&lt;TStartup&gt;()"] --> J["Llama SubscribeAsync&lt;TEvent,THandler&gt;\npor cada handler encontrado"]
```

### Ejemplo de uso en `Program.cs` / `Startup.cs`

```csharp
// Registro
builder.Services
    .AddEventBus()
    .AddEventsHandlers<MyStartup>();

// Suscripción (después de construir el host)
app.Services.SubscribeEventsHandlers<MyStartup>();
```

---

## Definir un evento y un handler

```csharp
// Evento: hereda de EventBase
public class UserCreatedEvent : EventBase
{
    public string UserId { get; set; }
    public string Email { get; set; }
}

// Handler: implementa IEventHandler<TEvent>
public class UserCreatedHandler : IEventHandler<UserCreatedEvent>
{
    public Task HandleAsync(UserCreatedEvent @event, CancellationToken token)
    {
        Console.WriteLine($"Usuario creado: {@event.UserId}");
        return Task.CompletedTask;
    }
}
```

---

## Publicar un evento manualmente

```csharp
// Inyectar IEventBus
public class UserService
{
    private readonly IEventBus _eventBus;

    public UserService(IEventBus eventBus) => _eventBus = eventBus;

    public async Task CreateUserAsync(string email, CancellationToken token)
    {
        // lógica de negocio...
        await _eventBus.PublichAsync(new UserCreatedEvent { UserId = "123", Email = email }, token);
    }
}
```

---

## Excepciones del dominio

| Excepción | Cuándo se lanza |
|---|---|
| `EventNotImplementedException` | No existe ninguna clase que implemente `IEventBus` en los ensamblados cargados |
| `EventIsNotRegisteredException` | Se consultan handlers de un evento que no está suscrito |
| `EventHandlerAlreadyRegisteredException` | Se intenta registrar el mismo handler dos veces para el mismo evento |
| `EventNotExistException` | El evento consultado no existe en el manager |
| `EventBusException` | Excepción base del dominio; las anteriores la heredan |

---

## Ejecutar pruebas

```bash
dotnet test
```

Los tests de integración y unitarios se encuentran en [tests/Microservice.Event.Bus.Test/](tests/Microservice.Event.Bus.Test/).

---

## Archivos clave

| Archivo | Descripción |
|---|---|
| [src/Microservice.Event.Bus/Abstractions/IEventBus.cs](src/Microservice.Event.Bus/Abstractions/IEventBus.cs) | Contrato principal del bus |
| [src/Microservice.Event.Bus/SubscriptionManager.cs](src/Microservice.Event.Bus/SubscriptionManager.cs) | Registro en memoria de suscripciones |
| [src/Microservice.Event.Bus/Internal/Queue/QueueService.cs](src/Microservice.Event.Bus/Internal/Queue/QueueService.cs) | Cola `ConcurrentQueue` thread-safe |
| [src/Microservice.Event.Bus/Internal/EventBusGackgroundService/EventBusBackgroundService.cs](src/Microservice.Event.Bus/Internal/EventBusGackgroundService/EventBusBackgroundService.cs) | Procesamiento asíncrono en segundo plano |
| [src/Microservice.Event.Bus/Extension/EventBusExtensions.cs](src/Microservice.Event.Bus/Extension/EventBusExtensions.cs) | Métodos de extensión para DI y auto-descubrimiento |
