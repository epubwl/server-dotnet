using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        private readonly EpubParser _epubParser;

        public EpubManager(EpubMetadataDbContext epubMetadataDbContext, IEpubBinaryDataStorage epubBinaryDataStorage, EpubParser epubParser)
        {
            _epubMetadataDbContext = epubMetadataDbContext;
            _epubBinaryDataStorage = epubBinaryDataStorage;
            _epubParser = epubParser;
        }

        public async Task<EpubMetadata?> GetEpubMetadataAsync(int epubId)
        {
            EpubMetadata? metadata = await _epubMetadataDbContext.EpubMetadata
                .Where(e => e.EpubId == epubId)
                .FirstOrDefaultAsync();
            return metadata;
        }

        public async Task<EpubMetadata> UpdateEpubMetadataAsync(EpubMetadata newMetadata)
        {
            EpubMetadata? metadata = await GetEpubMetadataAsync(newMetadata.EpubId);
            if (metadata is null)
            {
                throw new ArgumentException();
            }
            _epubMetadataDbContext.Entry(metadata).CurrentValues.SetValues(newMetadata);
            await _epubMetadataDbContext.SaveChangesAsync();
            return newMetadata;
        }

        public async Task<EpubMetadata> AddEpubAsync(string owner, Stream binaryStream)
        {
            await using (binaryStream)
            {
                var metadata = new EpubMetadata()
                {
                    Owner = owner
                };
                await _epubMetadataDbContext.AddAsync(metadata);
                await _epubMetadataDbContext.SaveChangesAsync();
                int epubId = metadata.EpubId;
                await _epubBinaryDataStorage.AddEpubAsync(epubId, binaryStream);
                
                await using (Stream epubStream = await GetEpubAsync(epubId))
                {
                    Stream coverStream;
                    string coverMimetype;
                    if (_epubParser.TryParse(epubStream, in metadata, out coverStream, out coverMimetype))
                    {
                        await UpdateEpubMetadataAsync(metadata);
                    }
                    await using (coverStream)
                    {
                        await _epubBinaryDataStorage.AddCoverAsync(epubId, coverStream, coverMimetype);
                    }
                    return metadata;
                }
            }
        }

        public async Task<Stream> GetEpubAsync(int epubId)
        {
            Stream binaryStream = await _epubBinaryDataStorage.GetEpubAsync(epubId);
            return binaryStream;
        }

        public async Task<EpubMetadata> ReplaceEpubAsync(int epubId, Stream binaryStream)
        {
            await using (binaryStream)
            {
                await _epubBinaryDataStorage.ReplaceEpubAsync(epubId, binaryStream);
                EpubMetadata? metadata = await GetEpubMetadataAsync(epubId);
                if (metadata is null)
                {
                    throw new ArgumentException();
                }
                return metadata;
            }
        }

        public async Task<EpubMetadata> DeleteEpubAsync(int epubId)
        {
            await _epubBinaryDataStorage.DeleteCoverAsync(epubId);
            await _epubBinaryDataStorage.DeleteEpubAsync(epubId);
            EpubMetadata? metadata = await GetEpubMetadataAsync(epubId);
            if (metadata is null)
            {
                throw new ArgumentException();
            }
            _epubMetadataDbContext.Remove(metadata);
            await _epubMetadataDbContext.SaveChangesAsync();
            return metadata;
        }

        public async Task DeleteAllEpubsFromOwner(string owner)
        {
            List<int> epubIds = await _epubMetadataDbContext.EpubMetadata
                .Where(e => e.Owner == owner)
                .Select(e => e.EpubId)
                .ToListAsync();
            foreach (int epubId in epubIds)
            {
                await DeleteEpubAsync(epubId);
            }
        }

        public async Task<Stream> GetEpubCoverAsync(int epubId)
        {
            return await _epubBinaryDataStorage.GetCoverAsync(epubId);
        }

        public async Task<string> GetEpubCoverMimetypeAsync(int epubId)
        {
            return await _epubBinaryDataStorage.GetCoverMimetypeAsync(epubId);
        }

        public async Task ReplaceEpubCoverAsync(int epubId, Stream binaryStream, string mimetype)
        {
            await _epubBinaryDataStorage.ReplaceCoverAsync(epubId, binaryStream, mimetype);
        }

        public async Task<IList<EpubMetadata>> Search(string owner)
        {
            List<EpubMetadata> metadatas = await _epubMetadataDbContext.EpubMetadata
                .Where(e => e.Owner == owner)
                .ToListAsync();
            return metadatas;
        }
    }
}