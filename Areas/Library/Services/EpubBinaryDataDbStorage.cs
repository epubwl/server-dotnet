using System.Data.Common;
using System.IO;
using System.Threading.Tasks;

namespace EpubWebLibraryServer.Areas.Library.Services
{
    public class EpubBinaryDataDbStorage : IEpubBinaryDataStorage
    {
        private readonly DbProviderFactory _dbProviderFactory;

        private readonly string _connectionString;

        public EpubBinaryDataDbStorage(DbProviderFactory dbProviderFactory, string connectionString)
        {
            this._dbProviderFactory = dbProviderFactory;
            this._connectionString = connectionString;
        }

        public async Task<long> AddEpubAsync(int epubId, Stream binaryStream)
        {
            using (DbConnection dbConnection = _dbProviderFactory.CreateConnection())
            {
                dbConnection.ConnectionString = _connectionString;
                dbConnection.Open();
                using (DbCommand dbCommand = _dbProviderFactory.CreateCommand())
                {
                    byte[] binaryData;
                    using (var memoryStream = new MemoryStream())
                    {
                        await binaryStream.CopyToAsync(memoryStream);
                        binaryData = memoryStream.ToArray();
                    }
                    dbCommand.Connection = dbConnection;
                    dbCommand.CommandText = "INSERT INTO EpubFiles (EpubId, BinaryData) VALUES (@EpubId, @EpubFileBinaryData)";
                    DbParameter epubIdParameter = dbCommand.CreateParameter();
                    epubIdParameter.ParameterName = "@EpubId";
                    epubIdParameter.Value = epubId;
                    DbParameter epubFileBinaryData = dbCommand.CreateParameter();
                    epubFileBinaryData.ParameterName = "@EpubFileBinaryData";
                    epubFileBinaryData.Value = binaryData;
                    dbCommand.Parameters.Add(epubIdParameter);
                    dbCommand.Parameters.Add(epubFileBinaryData);
                    await dbCommand.ExecuteNonQueryAsync();
                }
            }
            return 0;
        }
        
        public async Task<Stream> GetEpubAsync(int epubId)
        {
            using (DbConnection dbConnection = _dbProviderFactory.CreateConnection())
            {
                dbConnection.ConnectionString = _connectionString;
                dbConnection.Open();
                using (DbCommand dbCommand = _dbProviderFactory.CreateCommand())
                {
                    dbCommand.Connection = dbConnection;
                    dbCommand.CommandText = "SELECT BinaryData FROM EpubFiles WHERE EpubId=@EpubId";
                    DbParameter epubIdParameter = dbCommand.CreateParameter();
                    epubIdParameter.ParameterName = "@EpubId";
                    epubIdParameter.Value = epubId;
                    dbCommand.Parameters.Add(epubIdParameter);
                    DbDataReader dbDataReader = await dbCommand.ExecuteReaderAsync();
                    await dbDataReader.ReadAsync();
                    return dbDataReader.GetStream(0);
                }
            }
        }
    }
}