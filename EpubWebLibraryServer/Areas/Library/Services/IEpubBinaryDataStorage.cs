using System.IO;
using System.Threading.Tasks;

namespace EpubWebLibraryServer.Areas.Library.Services
{
    public interface IEpubBinaryDataStorage
    {
        Task AddEpubAsync(int epubId, Stream binaryStream);
        
        Task<Stream> GetEpubAsync(int epubId);

        Task ReplaceEpubAsync(int epubId, Stream binaryStream);

        Task DeleteEpubAsync(int epubId);

        Task AddCoverAsync(int epubId, Stream binaryStream, string mimetype);

        Task<Stream> GetCoverAsync(int epubId);

        Task<string> GetCoverMimetypeAsync(int epubId);

        Task ReplaceCoverAsync(int epubId, Stream binaryStream, string mimetype);

        Task DeleteCoverAsync(int epubId);
    }
}