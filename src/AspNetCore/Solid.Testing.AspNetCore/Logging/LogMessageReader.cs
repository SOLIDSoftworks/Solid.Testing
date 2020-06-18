using Microsoft.Extensions.Options;
using Solid.Testing.AspNetCore.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Testing.AspNetCore.Logging
{
    public class LogMessageReader
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
                    try
                    {
                        _options.OnLogMessage?.Invoke(message);
                    }
                    catch { }
                }
            }, TaskCreationOptions.LongRunning);
        }

        public bool MessagesAvailable => _channel.MessagesWaiting;
    }
}
