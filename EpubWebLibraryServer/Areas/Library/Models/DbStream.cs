using System.Data.Common;
using System.IO;

namespace EpubWebLibraryServer.Areas.Library.Models
{
    public class DbStream : Stream
    {
        private readonly DbConnection _dbConnection;

        private readonly DbCommand _dbCommand;

        private readonly DbDataReader _dbDataReader;

        private readonly Stream _stream;

        public DbStream(DbConnection dbConnection, DbCommand dbCommand, DbDataReader dbDataReader, Stream stream)
        {
            _dbConnection = dbConnection;
            _dbCommand = dbCommand;
            _dbDataReader = dbDataReader;
            _stream = stream;
        }

        public override bool CanRead { get => _stream.CanRead; }

        public override bool CanSeek { get => _stream.CanSeek; }

        public override bool CanWrite { get => _stream.CanWrite; }

        public override long Length { get => _stream.Length; }

        public override long Position { get => _stream.Position; set => _stream.Position = value; }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    _stream.Dispose();
                    _dbDataReader.Dispose();
                    _dbCommand.Dispose();
                    _dbConnection.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }
    }
}