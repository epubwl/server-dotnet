using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EpubWebLibraryServer.Areas.Library.Data;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Services
{
    public class EpubManager
    {
        private readonly EpubMetadataDbContext _epubMetadataDbContext;
        
        private readonly IEpubBinaryDataStorage _epubBinaryDataStorage;

        private readonly EpubMetadataParser _epubMetadataParser;

        public EpubManager(EpubMetadataDbContext epubMetadataDbContext, IEpubBinaryDataStorage epubBinaryDataStorage, EpubMetadataParser epubMetadataParser)
        {
            _epubMetadataDbContext = epubMetadataDbContext;
            _epubBinaryDataStorage = epubBinaryDataStorage;
            _epubMetadataParser = epubMetadataParser;
        }

        public async Task<EpubMetadata> GetEpubMetadataAsync(int epubId)
        {
            EpubMetadata metadata = await _epubMetadataDbContext.EpubMetadata
                .Where(e => e.EpubId == epubId)
                .FirstOrDefaultAsync();
            return metadata;
        }

        public async Task<EpubMetadata> UpdateEpubMetadataAsync(EpubMetadata newMetadata)
        {
            EpubMetadata metadata = await GetEpubMetadataAsync(newMetadata.EpubId);
            _epubMetadataDbContext.Entry(metadata).CurrentValues.SetValues(newMetadata);
            await _epubMetadataDbContext.SaveChangesAsync();
            return newMetadata;
        }

        public async Task<EpubMetadata> AddEpubAsync(string owner, Stream binaryStream)
        {
            using (binaryStream)
            {
                var metadata = new EpubMetadata()
                {
                    Owner = owner
                };
                await _epubMetadataDbContext.AddAsync(metadata);
                await _epubMetadataDbContext.SaveChangesAsync();
                int epubId = metadata.EpubId;
                await _epubBinaryDataStorage.AddEpubAsync(epubId, binaryStream);
                
                Stream epubStream = await GetEpubAsync(epubId);
                Stream coverStream;
                string coverMimetype;
                if (_epubMetadataParser.TryParse(epubStream, in metadata, out coverStream, out coverMimetype))
                {
                    await UpdateEpubMetadataAsync(metadata);
                }
                using (coverStream)
                {
                    await _epubBinaryDataStorage.AddCoverAsync(epubId, coverStream, coverMimetype);
                }
                return metadata;
            }
        }

        public async Task<Stream> GetEpubAsync(int epubId)
        {
            Stream binaryStream = await _epubBinaryDataStorage.GetEpubAsync(epubId);
            return binaryStream;
        }

        public async Task<EpubMetadata> ReplaceEpubAsync(int epubId, string owner, Stream binaryStream)
        {
            using (binaryStream)
            {
                await _epubBinaryDataStorage.ReplaceEpubAsync(epubId, binaryStream);
                EpubMetadata metadata = await GetEpubMetadataAsync(epubId);
                return metadata;
            }
        }

        public async Task<EpubMetadata> DeleteEpubAsync(int epubId)
        {
            EpubMetadata metadata = await GetEpubMetadataAsync(epubId);
            _epubMetadataDbContext.Remove(metadata);
            await _epubMetadataDbContext.SaveChangesAsync();
            await _epubBinaryDataStorage.DeleteEpubAsync(epubId);
            await _epubBinaryDataStorage.DeleteCoverAsync(epubId);
            return metadata;
        }
    }
}