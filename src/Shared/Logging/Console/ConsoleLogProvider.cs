using System.Runtime.CompilerServices;

namespace NoCrast.Shared.Logging
{
    public class ConsoleLogProvider : ILogProvider
    {
        public LogLevel Level { get; private set; }

        public ConsoleLogProvider(LogLevel level)
        {
            Level = level;
        }

        public ILog CreateLogger([CallerMemberName] string scope = null)
        {
            return new LogToConsole(this, Level, scope, false);
        }

    }
}
