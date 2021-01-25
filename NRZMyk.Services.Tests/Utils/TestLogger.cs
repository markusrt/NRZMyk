using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace NRZMyk.Services.Tests.Utils
{
    public class TestLogger<T> : ILogger<T>
    {
        public Dictionary<LogLevel, string> Messages { get; set; } = new Dictionary<LogLevel, string>();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Messages.Add(logLevel, state.ToString());
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}