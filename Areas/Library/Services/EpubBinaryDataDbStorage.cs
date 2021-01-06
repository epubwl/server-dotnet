using System.Collections.Generic;
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

        public async Task AddEpubAsync(int epubId, Stream binaryStream)
        {
            byte[] binaryData;
            using (var memoryStream = new MemoryStream())
            {
                await binaryStream.CopyToAsync(memoryStream);
                binaryData = memoryStream.ToArray();
            }
            string commandText = "INSERT INTO EpubFiles (EpubId, BinaryData) VALUES (@EpubId, @EpubFileBinaryData)";
            var parameters = new Dictionary<string, object>
            {
                ["@EpubId"] = epubId,
                ["@EpubFileBinaryData"] = binaryData
            };
            await ExecuteNonQueryAsync(commandText, parameters);
        }
        
        public async Task<Stream> GetEpubAsync(int epubId)
        {
            string commandText = "SELECT BinaryData FROM EpubFiles WHERE EpubId=@EpubId";
            var parameters = new Dictionary<string, object>
            {
                ["@EpubId"] = epubId
            };
            return await ExecuteStreamAsync(commandText, parameters);
        }

        public async Task ReplaceEpubAsync(int epubId, Stream binaryStream)
        {
            byte[] binaryData;
            using (var memoryStream = new MemoryStream())
            {
                await binaryStream.CopyToAsync(memoryStream);
                binaryData = memoryStream.ToArray();
            }
            string commandText = "UPDATE EpubFiles SET BinaryData=@EpubFileBinaryData WHERE EpubId=@EpubId";
            var parameters = new Dictionary<string, object>
            {
                ["@EpubId"] = epubId,
                ["@EpubFileBinaryData"] = binaryData
            };
            await ExecuteNonQueryAsync(commandText, parameters);
        }

        public async Task DeleteEpubAsync(int epubId)
        {
            string commandText = "DELETE FROM EpubFiles WHERE EpubId=@EpubId";
            var parameters = new Dictionary<string, object>
            {
                ["@EpubId"] = epubId
            };
            await ExecuteNonQueryAsync(commandText, parameters);
        }

        public async Task DeleteCoverAsync(int epubId)
        {
            string commandText = "DELETE FROM EpubCovers WHERE EpubId=@EpubId";
            var parameters = new Dictionary<string, object>
            {
                ["@EpubId"] = epubId
            };
            await ExecuteNonQueryAsync(commandText, parameters);
        }

        private async Task ExecuteNonQueryAsync(string commandText, IDictionary<string, object> parameters)
        {
            using (DbConnection dbConnection = _dbProviderFactory.CreateConnection())
            {
                dbConnection.ConnectionString = _connectionString;
                dbConnection.Open();
                using (DbCommand dbCommand = _dbProviderFactory.CreateCommand())
                {
                    dbCommand.Connection = dbConnection;
                    dbCommand.CommandText = commandText;
                    foreach (KeyValuePair<string, object> parameter in parameters)
                    {
                        DbParameter dbParameter = dbCommand.CreateParameter();
                        dbParameter.ParameterName = parameter.Key;
                        dbParameter.Value = parameter.Value;
                        dbCommand.Parameters.Add(dbParameter);
                    }
                    await dbCommand.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task<Stream> ExecuteStreamAsync(string commandText, IDictionary<string, object> parameters)
        {
            using (DbConnection dbConnection = _dbProviderFactory.CreateConnection())
            {
                dbConnection.ConnectionString = _connectionString;
                dbConnection.Open();
                using (DbCommand dbCommand = _dbProviderFactory.CreateCommand())
                {
                    dbCommand.Connection = dbConnection;
                    dbCommand.CommandText = commandText;
                    foreach (KeyValuePair<string, object> parameter in parameters)
                    {
                        DbParameter dbParameter = dbCommand.CreateParameter();
                        dbParameter.ParameterName = parameter.Key;
                        dbParameter.Value = parameter.Value;
                        dbCommand.Parameters.Add(dbParameter);
                    }
                    DbDataReader dbDataReader = await dbCommand.ExecuteReaderAsync();
                    await dbDataReader.ReadAsync();
                    return dbDataReader.GetStream(0);
                }
            }
        }
    }
}