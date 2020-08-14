using System;

namespace NoCrast.Shared.Logging
{
    public class LogToConsole : ILog
    {
        private DateTime timeStart;
        public LogLevel Level { get; }
        public string ScopeName { get; }

        public LogToConsole(LogLevel level, string scope, bool logConstructor)
        {
            timeStart = DateTime.Now;
            Level = level;
            ScopeName = scope;
            if (logConstructor && Level >= LogLevel.DEBUG && !String.IsNullOrEmpty(ScopeName))
            {
                Console.WriteLine($"--> begin of {ScopeName}");
            }
        }

        public ILog CreateScope(LogLevel level, string childScope)
        {
            var s = childScope;
            if (!String.IsNullOrEmpty(ScopeName))
            {
                s = $"{ScopeName}:{childScope}";
            }
            return new LogToConsole(level, s, true);
        }

        public void Dispose()
        {
            if (Level >= LogLevel.DEBUG)
            {
                var execTime = DateTime.Now - timeStart;
                Console.WriteLine($"<-- end of {ScopeName}, time:{execTime.TotalMilliseconds} msec");
            }
        }

        public void Line(LogLevel l, string message)
        {
            if (l <= Level)
            {
                if (String.IsNullOrEmpty(ScopeName))
                {
                    Console.WriteLine($"    {message}");
                }
                else
                {
                    Console.WriteLine($"    {ScopeName}: {message}");
                }
            }
        }
    }
}
