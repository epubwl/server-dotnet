using System.IO;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Services.EpubParsing
{
    public interface IEpubParser
    {
        bool TryParse(Stream epubStream, in EpubMetadata metadata, out Stream coverStream, out string coverMimetype);
    }
}