using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;

namespace backend.Services.Interfaces
{
    public interface IStorageConnectionFactory
    {
        Task<CloudBlobContainer> GetContainer();
    }
}
