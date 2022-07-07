using System.Xml.Linq;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Services.EpubParsing
{
    public interface IXDocumentEpubMetadataFieldParser
    {
        bool TryParse(XDocument opfDocument, in EpubMetadata metadata);
    }
}