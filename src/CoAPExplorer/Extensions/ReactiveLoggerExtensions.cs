using System;
using Microsoft.Extensions.Logging;

namespace CoAPExplorer.Extensions
{
    public static class SplatLocatorExtensions
    {
        public static Splat.IMutableDependencyResolver RegisterLogger(this Splat.IMutableDependencyResolver services, Type type)
        {
            var genericReactiveLogger = typeof(ReactiveLogger<>).MakeGenericType(type);
            var genericLogger = typeof(ILogger<>).MakeGenericType(type);

            services.Register(() => Activator.CreateInstance(genericReactiveLogger), genericLogger);
            return services;
        }

        public static Splat.IMutableDependencyResolver RegisterLogger<TType>(this Splat.IMutableDependencyResolver services)
        {
            services.Register(() => new ReactiveLogger<TType>(), typeof(ILogger<TType>));
            return services;
        }

        public static ILogger<TType> GetLogger<TType>(this Splat.IDependencyResolver services)
        {
            ILogger<TType> logger = (ILogger<TType>)services.GetService(typeof(ILogger<TType>));
            if (logger == null)
            {
                if (services is Splat.IMutableDependencyResolver mutableServices)
                {
                    RegisterLogger<TType>(mutableServices);
                    logger = (ILogger<TType>)services.GetService(typeof(ILogger<TType>));
                }
                else
                {
                    logger = new ReactiveLogger<TType>();
                }
            }
            return logger;
        }

        public static ILogger GetLogger(this Splat.IDependencyResolver services, Type type)
        {
            var genericLogger = typeof(ILogger<>).MakeGenericType(type);

            ILogger logger = (ILogger)services.GetService(genericLogger);
            if (logger == null)
            {
                if (services is Splat.IMutableDependencyResolver mutableServices)
                {
                    RegisterLogger(mutableServices, type);
                    logger = (ILogger)services.GetService(genericLogger);
                }
                else
                {
                    var genericReactiveLogger = typeof(ReactiveLogger<>).MakeGenericType(type);
                    logger = (ILogger)Activator.CreateInstance(genericReactiveLogger);
                }
            }
            return logger;
        }
    }

    public class ReaciveLoggerFactory
    {
        public ILogger<T> CreateLogger<T>()
        {
            return new ReactiveLogger<T>();
        }

        public ILogger CreateLogger(Type type)
        {
            var genericReactiveLogger = typeof(ReactiveLogger<>).MakeGenericType(type);
            return (ILogger)Activator.CreateInstance(genericReactiveLogger);
        }
    }

    public class ReactiveLogger<TClass> : ILogger<TClass>
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Trace)
                return false;
            return true; // throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message) && exception == null)
                return;

            // Get Splat's ILogManager
            var factory = (Splat.ILogManager)Splat.Locator.Current.GetService(typeof(Splat.ILogManager))
                ?? throw new Exception($"{nameof(Splat.ILogManager)} was not found. Please ensure your dependency resolver is configured correctly.");

            // Get a logger to reprecent our TClass
            var logger = factory.GetLogger(typeof(TClass));

            switch (logLevel)
            {
                case LogLevel.Trace:
                    break;
                case LogLevel.Debug:
                    if (exception != null)
                        logger.DebugException($"[{eventId.Id}]: {message}", exception);
                    else
                        logger.Debug($"[{eventId.Id}]: {message}");
                    break;
                case LogLevel.Information:
                    if (exception != null)
                        logger.InfoException($"[{eventId.Id}]: {message}", exception);
                    else
                        logger.Info($"[{eventId.Id}]: {message}");
                    break;
                case LogLevel.Warning:
                    if (exception != null)
                        logger.WarnException($"[{eventId.Id}]: {message}", exception);
                    else
                        logger.Warn($"[{eventId.Id}]: {message}");
                    break;
                case LogLevel.Error:
                    if (exception != null)
                        logger.ErrorException($"[{eventId.Id}]: {message}", exception);
                    else
                        logger.Error($"[{eventId.Id}]: {message}");
                    break;
                case LogLevel.Critical:
                    if (exception != null)
                        logger.FatalException($"[{eventId.Id}]: {message}", exception);
                    else
                        logger.Fatal($"[{eventId.Id}]: {message}");
                    break;
                case LogLevel.None:
                    break;
            }
        }
    }
}