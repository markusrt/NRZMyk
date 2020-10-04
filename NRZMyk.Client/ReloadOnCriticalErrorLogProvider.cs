using System;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace NRZMyk.Client
{
    internal class ReloadOnCriticalErrorLogProvider : ILoggerProvider
    {
        private const string WebAssemblyLogger = "Microsoft.AspNetCore.Components.WebAssembly.Rendering.WebAssemblyRenderer";

        private ReloadBaseUriLogger _logger;

        public ReloadOnCriticalErrorLogProvider(NavigationManager navigationManager)
        {
            _logger = new ReloadBaseUriLogger(navigationManager);
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (categoryName == WebAssemblyLogger)
            {
                return _logger;
            }
            return NullLogger.Instance;
        }

        public void Dispose()
        {
            _logger = null;
        }

        private class ReloadBaseUriLogger : ILogger
        {
            private readonly NavigationManager _navigationManager;

            public ReloadBaseUriLogger(NavigationManager navigationManager)
            {
                _navigationManager = navigationManager;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return logLevel == LogLevel.Critical;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!IsEnabled(logLevel) || eventId.Id != 100) return;

                Console.WriteLine("Reload base uri after critical error");

                _navigationManager.NavigateTo(_navigationManager.BaseUri, true);
            }
        }
    }
}