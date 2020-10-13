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

        public ReloadOnCriticalErrorLogProvider(NavigationManager navigationManager, string errorContains)
        {
            _logger = new ReloadBaseUriLogger(navigationManager, errorContains);
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
            private readonly string _errorContains;

            public ReloadBaseUriLogger(NavigationManager navigationManager, string errorContains)
            {
                _navigationManager = navigationManager;
                _errorContains = errorContains;
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
                if(!formatter(state, exception).Contains(_errorContains)) return;

                _navigationManager.NavigateTo(_navigationManager.BaseUri, true);
            }
        }
    }
}