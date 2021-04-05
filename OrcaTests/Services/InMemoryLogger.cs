using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrcaTests.Services
{

    public class InMemoryLogger<A> : ILogger<A>
    {

        public IList<string> LogLines { get; private set; } = new List<string>();

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var log = formatter.Invoke(state, exception);
            LogLines.Add(log);

        }
    }
}
