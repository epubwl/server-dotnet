using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Services
{
    public class DbStreamFactory
    {
        private readonly DbProviderFactory _dbProviderFactory;

        private readonly string _connectionString;

        public DbStreamFactory(DbProviderFactory dbProviderFactory, string connectionString)
        {
            _dbProviderFactory = dbProviderFactory;
            _connectionString = connectionString;
        }

        public async Task<DbStream> CreateDbStreamAsync(string commandText, IDictionary<string, object> parameters)
        {
            DbConnection dbConnection = _dbProviderFactory.CreateConnection();
            dbConnection.ConnectionString = _connectionString;
            await dbConnection.OpenAsync();

            DbCommand dbCommand = _dbProviderFactory.CreateCommand();
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