using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Services
{
    interface IEpubParsingStrategy
    {
        bool TryParseCover(ZipArchive zipArchive, XDocument opfDocument, string opfPath, out Stream coverStream, out string coverMimetype);

        bool TryParseCreators(XDocument opfDocument, in EpubMetadata metadata);

        bool TryParseDate(XDocument opfDocument, in EpubMetadata metadata);

        bool TryParseTitle(XDocument opfDocument, in EpubMetadata metadata);
    }
}