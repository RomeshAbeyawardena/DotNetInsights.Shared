using DotNetInsights.Shared.Contracts;
using System;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace DotNetInsights.Shared.Services
{
    public abstract class DefaultServiceBroker : IServiceBroker
    {
        public abstract IEnumerable<Assembly> GetAssemblies {get;}
        public static Assembly DefaultAssembly => Assembly.GetAssembly(typeof(DefaultAppHost));
        
        public void RegisterServiceAssemblies(IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, params Assembly[] assemblies)
        {
            if(services == null)
                throw new ArgumentNullException(nameof(services));

            foreach(var assembly in assemblies)
            {
                var assemblyTypes = assembly.GetTypes();
                var serviceRegistrationTypes = assemblyTypes
                    .Where(type => type.GetInterface(nameof(IServiceRegistration)) != null);

                var eventHandlerTypes = assemblyTypes.Where(type => type.GetInterfaces().Any(a => a.IsAssignableFrom(typeof(IEventHandler))));
                var notificationSubscriberTypes = assemblyTypes.Where(type => type.GetInterfaces()
                    .Any(a => a.IsAssignableFrom(typeof(INotificationSubscriber))));
                var validatorTypes = assemblyTypes.Where(type => type.GetInterfaces()
                    .Any(a => a.IsAssignableFrom(typeof(IValidator)) && !type.IsAbstract));

                RegisterEventHandlerTypes(services, eventHandlerTypes, serviceLifetime);
                RegisterSubscriberTypes(services, notificationSubscriberTypes, serviceLifetime);
                RegisterValidators(services, validatorTypes, serviceLifetime);

                foreach (var item in serviceRegistrationTypes)
                {
                   var serviceRegistration = Activator.CreateInstance(item) as IServiceRegistration;
                    serviceRegistration.RegisterServices(services);
                }
            }
        }

        private void RegisterEventHandlerTypes(IServiceCollection services, IEnumerable<Type> eventHandlerTypes, ServiceLifetime serviceLifetime)
        {
            foreach(var eventHandlerType in eventHandlerTypes)
            {
                if(eventHandlerType.IsAbstract)
                    continue;
                
                var genericServiceType = typeof(IEventHandler<>);
                
                var genericArguments = eventHandlerType.GetInterfaces().FirstOrDefault().GetGenericArguments();
                var serviceType = genericServiceType.MakeGenericType(genericArguments);
                Console.WriteLine(serviceType.FullName);
                Console.WriteLine(eventHandlerType.FullName);

                var newDescriptor = new ServiceDescriptor(serviceType, eventHandlerType, serviceLifetime);

                services.Add(newDescriptor);

                if(!services.Contains(newDescriptor))
                    throw new Exception();
            }
        }

        private void RegisterSubscriberTypes(IServiceCollection services, IEnumerable<Type> subscriberTypes, ServiceLifetime serviceLifetime)
        {
            var eventHandlerTypeListTypes = new List<Type>();
            foreach(var eventHandlerType in subscriberTypes)
            {
                if(eventHandlerType.IsAbstract)
                    continue;
                
                var genericServiceType = typeof(INotificationSubscriber<>);
                var genericArguments = eventHandlerType.GetInterfaces().FirstOrDefault().GetGenericArguments();
                var gServiceType = genericServiceType.MakeGenericType(genericArguments);
                eventHandlerTypeListTypes.Add(gServiceType);
                services.Add(new ServiceDescriptor(gServiceType, eventHandlerType, ServiceLifetime.Singleton));
            }

            services.AddSingleton<IList<Type>>(eventHandlerTypeListTypes);
        }

        private void RegisterValidators(IServiceCollection services, IEnumerable<Type> validatorTypes, ServiceLifetime serviceLifetime)
        {
            foreach(var validatorType in validatorTypes)
            {
                var genericServiceType = typeof(IValidator<>);
                var genericArguments = validatorType.GetInterfaces().FirstOrDefault().GetGenericArguments();
                var gServiceType = genericServiceType.MakeGenericType(genericArguments);

                Console.WriteLine(gServiceType.FullName);
                Console.WriteLine(validatorType.FullName);
                services.Add(new ServiceDescriptor(gServiceType, validatorType, serviceLifetime));
            }
        }
    }
}
