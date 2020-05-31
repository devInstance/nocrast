
namespace NoCrast.Shared.Logging
{
    public interface ILogProvider
    {
        ILog CreateLogger(string scope);
    }
}
