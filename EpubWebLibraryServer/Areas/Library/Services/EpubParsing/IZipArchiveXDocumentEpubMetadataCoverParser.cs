using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

namespace EpubWebLibraryServer.Areas.Library.Services.EpubParsing
{
    public interface IZipArchiveXDocumentEpubMetadataCoverParser
    {
        bool TryParse(ZipArchive zipArchive, XDocument opfDocument, string opfPath, out Stream coverStream, out string coverMimetype);
    }
}