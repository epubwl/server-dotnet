using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Services
{
    public class EpubBinaryDataDbStorage : IEpubBinaryDataStorage
    {
        private readonly DbProviderFactory _dbProviderFactory;

        private readonly string _connectionString;

        public EpubBinaryDataDbStorage(DbProviderFactory dbProviderFactory, string connectionString)
        {
            _dbProviderFactory = dbProviderFactory;
            _connectionString = connectionString;
        }

        public async Task AddEpubAsync(int epubId, Stream binaryStream)
        {
            byte[] binaryData;
            await using (var memoryStream = new MemoryStream())
            {
                await binaryStream.CopyToAsync(memoryStream);
                binaryData = memoryStream.ToArray();
            }
            string commandText = "INSERT INTO \"EpubFiles\" (\"EpubId\", \"BinaryData\") VALUES (@EpubId, @EpubFileBinaryData)";
            var parameters = new Dictionary<string, object>
            {
                ["@EpubId"] = epubId,
                ["@EpubFileBinaryData"] = binaryData
            };
            await ExecuteNonQueryAsync(commandText, parameters);
        }
        
        public async Task<Stream> GetEpubAsync(int epubId)
        {
            string commandText = "SELECT \"BinaryData\" FROM \"EpubFiles\" WHERE \"EpubId\"=@EpubId";
            var parameters = new Dictionary<string, object>
            {
                ["@EpubId"] = epubId
            };
            return await ExecuteStreamAsync(commandText, parameters);
        }

        public async Task ReplaceEpubAsync(int epubId, Stream binaryStream)
        {
            byte[] binaryData;
            await using (var memoryStream = new MemoryStream())
            {
                await binaryStream.CopyToAsync(memoryStream);
                binaryData = memoryStream.ToArray();
            }
            string commandText = "UPDATE \"EpubFiles\" SET \"BinaryData\"=@EpubFileBinaryData WHERE \"EpubId\"=@EpubId";
            var parameters = new Dictionary<string, object>
            {
                ["@EpubId"] = epubId,
                ["@EpubFileBinaryData"] = binaryData
            };
            await ExecuteNonQueryAsync(commandText, parameters);
        }

        public async Task DeleteEpubAsync(int epubId)
        {
            string commandText = "DELETE FROM \"EpubFiles\" WHERE \"EpubId\"=@EpubId";
            var parameters = new Dictionary<string, object>
            {
                ["@EpubId"] = epubId
            };
            await ExecuteNonQueryAsync(commandText, parameters);
        }

        public async Task AddCoverAsync(int epubId, Stream binaryStream, string mimetype)
        {
            byte[] binaryData;
            await using (var memoryStream = new MemoryStream())
            {
                await binaryStream.CopyToAsync(memoryStream);
                binaryData = memoryStream.ToArray();
            }
            string commandText = "INSERT INTO \"EpubCovers\" (\"EpubId\", \"BinaryData\", \"Mimetype\") VALUES (@EpubId, @EpubCoverBinaryData, @EpubCoverMimetype)";
            var parameters = new Dictionary<string, object>
            {
                ["@EpubId"] = epubId,
                ["@EpubCoverBinaryData"] = binaryData,
                ["@EpubCoverMimetype"] = mimetype
            };
            await ExecuteNonQueryAsync(commandText, parameters);
        }

        public async Task<Stream> GetCoverAsync(int epubId)
        {
            string commandText = "SELECT \"BinaryData\", \"Mimetype\" FROM \"EpubCovers\" WHERE \"EpubId\"=@EpubId";
            var parameters = new Dictionary<string, object>
            {
                ["@EpubId"] = epubId
            };
            return await ExecuteStreamAsync(commandText, parameters);
        }

        public async Task<string> GetCoverMimetypeAsync(int epubId)
        {
            string unknownMimetype = EpubMimeTypes.Application.OctetStream;
            string commandText = "SELECT \"Mimetype\" FROM \"EpubCovers\" WHERE \"EpubId\"=@EpubId";
            var parameters = new Dictionary<string, object>
            {
                ["@EpubId"] = epubId
            };
            string mimetype;
            await using (DbConnection? dbConnection = _dbProviderFactory.CreateConnection())
            {
                if (dbConnection is null)
                {
                    return unknownMimetype;
                }
                dbConnection.ConnectionString = _connectionString;
                dbConnection.Open();
                await using (DbCommand? dbCommand = _dbProviderFactory.CreateCommand())
                {
                    if (dbCommand is null)
                    {
                        return unknownMimetype;
                    }
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
                    mimetype = dbDataReader.GetString(0);
                }
            }
            return mimetype;
        }

        public async Task ReplaceCoverAsync(int epubId, Stream binaryStream, string mimetype)
        {
            byte[] binaryData;
            await using (var memoryStream = new MemoryStream())
            {
                await binaryStream.CopyToAsync(memoryStream);
                binaryData = memoryStream.ToArray();
            }
            string commandText = "UPDATE \"EpubCovers\" SET \"BinaryData\"=@EpubCoverBinaryData, \"Mimetype\"=@EpubCoverMimetype WHERE \"EpubId\"=@EpubId";
            var parameters = new Dictionary<string, object>
            {
                ["@EpubId"] = epubId,
                ["@EpubCoverBinaryData"] = binaryData,
                ["@EpubCoverMimetype"] = mimetype
            };
            await ExecuteNonQueryAsync(commandText, parameters);
        }

        public async Task DeleteCoverAsync(int epubId)
        {
            string commandText = "DELETE FROM \"EpubCovers\" WHERE \"EpubId\"=@EpubId";
            var parameters = new Dictionary<string, object>
            {
                ["@EpubId"] = epubId
            };
            await ExecuteNonQueryAsync(commandText, parameters);
        }

        private async Task ExecuteNonQueryAsync(string commandText, IDictionary<string, object> parameters)
        {
            await using (DbConnection? dbConnection = _dbProviderFactory.CreateConnection())
            {
                if (dbConnection is null)
                {
                    throw new InvalidOperationException();
                }
                dbConnection.ConnectionString = _connectionString;
                dbConnection.Open();
                await using (DbCommand? dbCommand = _dbProviderFactory.CreateCommand())
                {
                    if (dbCommand is null)
                    {
                        throw new InvalidOperationException();
                    }
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

        private async Task<DbStream> ExecuteStreamAsync(string commandText, IDictionary<string, object> parameters)
        {
            await using (DbConnection? dbConnection = _dbProviderFactory.CreateConnection())
            {
                if (dbConnection is null)
                {
                    throw new InvalidOperationException();
                }
                dbConnection.ConnectionString = _connectionString;
                await dbConnection.OpenAsync();
                
                await using (DbCommand? dbCommand = _dbProviderFactory.CreateCommand())
                {
                    if (dbCommand is null)
                    {
                        throw new InvalidOperationException();
                    }
                    dbCommand.CommandText = commandText;
                    dbCommand.Connection = dbConnection;

                    foreach (KeyValuePair<string, object> parameter in parameters)
                    {
                        DbParameter dbParameter = dbCommand.CreateParameter();
                        dbParameter.ParameterName = parameter.Key;
                        dbParameter.Value = parameter.Value;
                        dbCommand.Parameters.Add(dbParameter);
                    }

                    DbDataReader dbDataReader = await dbCommand.ExecuteReaderAsync();

                    Stream stream = await dbDataReader.ReadAsync() && !(await dbDataReader.IsDBNullAsync(0))
                        ? dbDataReader.GetStream(0)
                        : Stream.Null;

                    return new DbStream(dbConnection, dbCommand, dbDataReader, stream);
                }
            }
            
        }
    }
}