using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Logging
{
    internal class ChannelLogger : ILogger
    {
        private LogMessageChannel _channel;
        private string _category;

        public ChannelLogger(LogMessageChannel channel, string category)
        {
            _channel = channel;
            _category = category;
        }

        public IDisposable BeginScope<TState>(TState state)
            => new DelegateScopeOutput<TState>(state, LogScopeOutput);

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) 
            => Log<TState>(logLevel.ToString().ToLower(), eventId, state, exception, formatter);
        private void Log<TState>(string logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = $"[{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}][{logLevel.PadRight(12)}] - {formatter(state, exception)} ({_category})";
            if (exception != null)
                message = message + Environment.NewLine + exception.ToString();
            _channel.Enqueue(message);
        }

        private void LogScopeOutput<TState>(string logLevel, TState state)
            => Log(logLevel, 0, state, null, (s, _) => s.ToString());
    }
}
