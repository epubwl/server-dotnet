using System.Xml.Linq;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Services.EpubParsing
{
    public class XDocumentEpubMetadataTitleParser : IXDocumentEpubMetadataFieldParser
    {
        public bool TryParse(XDocument opfDocument, in EpubMetadata metadata)
        {
            XNamespace opfNamespace = EpubXmlNamespaces.Opf;
            XNamespace dcNamespace = EpubXmlNamespaces.Dc;
            string? title = opfDocument
                ?.Element(opfNamespace + "package")
                ?.Element(opfNamespace + "metadata")
                ?.Element(dcNamespace + "title")
                ?.Value;
            if (title is null)
            {
                return false;
            }
            metadata.Title = title;
            return true;
        }
    }
}