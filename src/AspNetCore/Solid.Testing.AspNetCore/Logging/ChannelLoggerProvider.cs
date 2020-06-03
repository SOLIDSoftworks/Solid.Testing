using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Logging
{
    internal class ChannelLoggerProvider : ILoggerProvider
    {
        private LogMessageChannel _channel;

        public ChannelLoggerProvider(LogMessageChannel channel)
        {
            _channel = channel;
        }
        public ILogger CreateLogger(string categoryName) => new ChannelLogger(_channel, categoryName);

        public void Dispose()
        {
        }
    }
}
