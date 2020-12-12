
namespace NoCrast.Shared.Logging
{
    public interface ILogProvider
    {
        LogLevel Level { get; }

        ILog CreateLogger(string scope);
    }
}
