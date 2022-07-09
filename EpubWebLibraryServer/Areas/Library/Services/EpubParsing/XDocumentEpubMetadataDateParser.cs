using System;
using System.Globalization;
using System.Xml.Linq;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Services.EpubParsing
{
    public class XDocumentEpubMetadataDateParser : IXDocumentEpubMetadataFieldParser
    {
        public bool TryParse(XDocument opfDocument, in EpubMetadata metadata)
        {
            XNamespace opfNamespace = EpubXmlNamespaces.Opf;
            XNamespace dcNamespace = EpubXmlNamespaces.Dc;
            string? date = opfDocument
                ?.Element(opfNamespace + "package")
                ?.Element(opfNamespace + "metadata")
                ?.Element(dcNamespace + "date")
                ?.Value;
            if (date is null)
            {
                return false;
            }
            try
            {
                metadata.Date = DateTime.Parse(date, CultureInfo.InvariantCulture).ToUniversalTime();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}