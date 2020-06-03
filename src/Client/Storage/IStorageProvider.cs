using NoCrast.Client.Model;
using System.Threading.Tasks;

namespace NoCrast.Client.Storage
{
    public interface IStorageProvider
    {
        Task<bool> SaveAsync(NoCrastData value);
        Task<NoCrastData> ReadAsync(); //TODO: hide
    }
}
