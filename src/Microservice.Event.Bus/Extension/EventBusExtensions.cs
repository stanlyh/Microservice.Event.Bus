using CodeDesignPlus.Core.Abstractions;
using Microservice.Event.Bus.Abstractions;
using Microservice.Event.Bus.Exceptions;
using Microservice.Event.Bus.Internal.EventBusGackgroundService;
using Microservice.Event.Bus.Internal.Queue;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Microservice.Event.Bus.Extension
{
    /// <summary>
    /// Extension methods for adding events handler to an Microsoft.Extensions.DependencyInjection.IServiceCollection.
    /// </summary>
    public static class EventBusExtensions
    {
        /// <summary>
        /// Adds the services of the type <see cref="IEventBus" and <see cref="ISubscriptionManager"/>/>
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
        /// <returns>A reference to this instance after the operations has completed.</returns>
        public static IServiceCollection AddEventBuss(this IServiceCollection services)
        {
            services.AddSingleton<ISubscriptionManager, SubscriptionManager>();

            var eventBus = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => typeof(IEventBus).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract && !x.IsInterface);

            if (eventBus == null)
                throw new EventNotImplementedException();

            services.AddSingleton(typeof(IEventBus), eventBus);

            return services;
        }

        /// <summary>
        /// Adds the event handlers that implement the IEventHandler interface
        /// </summary>
        /// <typeparam name="TStarupLogic">Implementation of the IStarupService type</typeparam>
        /// <param name="services">A reference to this instance after the operation has completed.</param>
        /// <returns></returns>
        public static IServiceCollection AddEventsHandlers<TStarupLogic>(this IServiceCollection services)
            where TStarupLogic : IStartupServices
        {
            var eventHandlers = GetEventHandlers<TStarupLogic>();

            foreach (var eventHandler in eventHandlers)
            {
                var interfaceEventHandlerGeneric = eventHandler.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>));

                var eventType = interfaceEventHandlerGeneric?.GetGenericArguments().FirstOrDefault(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(EventBase)));

                if (interfaceEventHandlerGeneric != null && eventType != null)
                {
                    var queueServiceType = typeof(IQueueService<,>).MakeGenericType(eventHandler, eventType);
                    var queueServiceImplementationType = typeof(QueueService<,>).MakeGenericType(eventHandler, eventType);

                    var hostServiceType = typeof(IEventBusBackgroundService<,,>).MakeGenericType(queueServiceImplementationType, eventHandler, eventType);
                    var hostServiceImplementationType = typeof(IEventBusBackgroundService<,,>).MakeGenericType(queueServiceImplementationType, eventHandler, eventType);

                    services.AddSingleton(queueServiceType, queueServiceImplementationType);
                    services.AddTransient(hostServiceType, hostServiceImplementationType);
                    services.AddTransient(eventHandler);
                }
            }

            return services;
        }

        /// <summary>
        /// Suscribe all implementation of the type IEventHandler
        /// </summary>
        /// <typeparam name="TStarupLogic">Implementation of the IStartupServices type</typeparam>
        /// <param name="provider">Provider of services</param>
        /// <returns>The service provider</returns>
        public static IServiceProvider SubscribeEventsHandlers<TStarupLogic>(this IServiceProvider provider)
            where TStarupLogic : IStartupServices
        {
            var eventBus = provider.GetRequiredService<IEventBus>();

            var typeEventBus = eventBus.GetType();

            var eventsHandlers = GetEventHandlers<TStarupLogic>();

            foreach (var eventHandler in eventsHandlers)
            {
                var interfaceEventHandlerGeneric = eventHandler.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>));

                if (interfaceEventHandlerGeneric != null)
                {
                    var eventType = interfaceEventHandlerGeneric.GetGenericArguments().FirstOrDefault(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(EventBase)));

                    if (!eventType.IsGenericParameter)
                    {
                        var methodSubscribe = typeEventBus.GetMethods().FirstOrDefault(x => x.Name == nameof(IEventBus.SubscribeAsync) && x.IsGenericMethod);

                        var methodGeneric = methodSubscribe.MakeGenericMethod(eventType, eventHandler);

                        (methodGeneric.Invoke(eventBus, null) as Task).ConfigureAwait(false);
                    }
                }
            }

            return provider;
        }

        /// <summary>
        /// Determines wheter an instance of a specified type can be assigned to a variable of the current type.
        /// </summary>
        /// <param name="type">Current type.</param>
        /// <param name="inteface">The type to compare with the current type.</param>
        /// <returns>Returns true if type implemented <paramref name="inteface"/></returns>
        public static bool IsAssignableGenericFrom(this Type type, Type @inteface)
        {
            return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == @inteface);
        }

        /// <summary>
        /// Escanea y retorna las clases que implemntan la interfaz <see cref="IEventHandler{TEvent}"/>
        /// </summary>
        /// <typeparam name="TStartupLogic">Clase de inicio que iplementa la interfaz <see cref="IStarupServices"/></typeparam>
        /// <returns>Return a list of type</returns>
        public static List<Type> GetEventHandlers<TStartupLogic>() where TStartupLogic : IStartupServices
        {
            return Assembly.GetAssembly(typeof(TStartupLogic))
                .GetTypes()
                .Where(x =>
                    x.IsClass &&
                    x.IsAssignableGenericFrom(typeof(IEventHandler<>))
                ).ToList();
        }
    }
}
