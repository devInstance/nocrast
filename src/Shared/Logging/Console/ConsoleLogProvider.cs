using System.Runtime.CompilerServices;

namespace NoCrast.Shared.Logging
{
    public class ConsoleLogProvider : ILogProvider
    {
        private LogLevel _Level;

        public ConsoleLogProvider(LogLevel level)
        {
            _Level = level;
        }

        public ILog CreateLogger([CallerMemberName] string scope = null)
        {
            return new LogToConsole(_Level, scope, false);
        }

    }
}
