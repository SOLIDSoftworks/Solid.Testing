using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Solid.Testing.AspNetCore.Extensions.XUnit.Logging
{
    internal class XUnitLoggerProvider : ILoggerProvider
    {
        private IServiceProvider _root;

        public XUnitLoggerProvider(IServiceProvider root)
        {
            _root = root;
        }
        public ILogger CreateLogger(string categoryName) => new XUnitLogger(categoryName, _root.GetService<ITestOutputHelper>());

        public void Dispose()
        {
        }
    }
}
