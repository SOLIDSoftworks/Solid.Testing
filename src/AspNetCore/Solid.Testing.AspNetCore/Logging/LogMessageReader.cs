using Microsoft.Extensions.Options;
using Solid.Testing.AspNetCore.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Testing.AspNetCore.Logging
{
    internal class LogMessageReader
    {
        private AspNetCoreHostOptions _options;
        private IServiceProvider _services;
        private LogMessageChannel _channel;

        public LogMessageReader(IServiceProvider services, LogMessageChannel channel, IOptionsMonitor<AspNetCoreHostOptions> monitor)
        {
            _options = monitor.CurrentValue;
            _services = services;
            _channel = channel;
        }

        public void Start()
        {
            _ = Task.Factory.StartNew(async () =>
            {
                while (!_channel.Completed)
                {
                    var message = await _channel.ReadAsync();
                    _options.OnLogMessage?.Invoke(_services, message);
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}
