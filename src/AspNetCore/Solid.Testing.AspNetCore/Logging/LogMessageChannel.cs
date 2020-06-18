using Microsoft.Extensions.Options;
using Solid.Testing.AspNetCore.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Solid.Testing.AspNetCore.Logging
{
    public class LogMessageChannel : IDisposable
    {
        private Channel<LogMessageContext> _channel;
        private int _messages = 0;

        public LogMessageChannel()
        {
            var options = new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            };
            _channel = Channel.CreateUnbounded<LogMessageContext>(options);
            _channel.Reader.Completion.ContinueWith(_ => Completed = true);
        }

        public bool Completed { get; private set; }

        public void Enqueue(LogMessageContext message)
        {
            if (_channel.Writer.TryWrite(message))
                _messages++;
        }

        public ValueTask<LogMessageContext> ReadAsync()
        {
            var read = _channel.Reader.TryRead(out var context);
            if(read)
                _messages--;
            return new ValueTask<LogMessageContext>(context);
        }
             
        public void Dispose() => _ = _channel.Writer.TryComplete();

        public bool MessagesWaiting => _messages > 0;
    }
}
