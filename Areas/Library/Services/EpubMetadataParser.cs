using System.IO;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Services
{
    public class EpubMetadataParser
    {
        public bool TryParse(Stream epubStream, in EpubMetadata metadata, out Stream coverStream, out string coverMimetype)
        {
            using (epubStream)
            {
                coverStream = Stream.Null;
                coverMimetype = "";
                return false;
            }
        }
    }
}