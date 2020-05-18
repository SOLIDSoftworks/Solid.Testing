using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace Solid.Testing.AspNetCore.Extensions.XUnit.Logging
{
    internal class XUnitLogger : ILogger
    {
        private string _categoryName;
        private ITestOutputHelper _output;

        public XUnitLogger(string categoryName, ITestOutputHelper output)
        {
            _categoryName = categoryName;
            _output = output;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            if (_output == null) return NullScope.Instance;
            return new DelegateScopeOutput<TState>(state, (l, s) => Log<TState>(l, new EventId(0), s, null, (st, _) => st.ToString()));
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) => Log<TState>(GetShortString(logLevel), eventId, state, exception, formatter);
        private void Log<TState>(string logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (_output == null) return;
            var message = $"[{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}][{logLevel}] - {formatter(state, exception)} ({_categoryName})";
            if (exception != null)
                message = message + Environment.NewLine + exception.ToString();
            _output.WriteLine(message);
        }

        private string GetShortString(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Information: return "Info ";
                case LogLevel.Critical: return "Crit ";
                case LogLevel.Warning: return "Warn ";
                case LogLevel.None: return level.ToString() + " ";
                case LogLevel.Debug:
                case LogLevel.Error:
                case LogLevel.Trace:
                default: return level.ToString();
            }
        }
        class NullScope : IDisposable
        {
            private NullScope() { }
            public static NullScope Instance => new NullScope();
            public void Dispose()
            {
            }
        }
    }
}
