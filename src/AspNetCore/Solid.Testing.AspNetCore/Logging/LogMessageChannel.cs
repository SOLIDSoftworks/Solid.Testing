using Microsoft.Extensions.Options;
using Solid.Testing.AspNetCore.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Solid.Testing.AspNetCore.Logging
{
    internal class LogMessageChannel : IDisposable
    {
        private Channel<string> _channel;

        public LogMessageChannel()
        {
            var options = new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            };
            _channel = Channel.CreateUnbounded<string>(options);
            _channel.Reader.Completion.ContinueWith(_ => Completed = true);
        }

        public bool Completed { get; private set; }

        public void Enqueue(string message) => _ = _channel.Writer.TryWrite(message);

        public ValueTask<string> ReadAsync() => _channel.Reader.ReadAsync();
             
        public void Dispose() => _ = _channel.Writer.TryComplete();
    }
}
