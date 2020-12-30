using System.IO;
using System.Threading.Tasks;

namespace EpubWebLibraryServer.Areas.Library.Services
{
    public interface IEpubBinaryDataStorage
    {
        Task<long> AddEpubAsync(int epubId, Stream binaryStream);
        
        Task<Stream> GetEpubAsync(int epubId);
    }
}