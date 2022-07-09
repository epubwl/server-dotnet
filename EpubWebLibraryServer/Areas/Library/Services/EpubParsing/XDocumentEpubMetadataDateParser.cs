using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            DateTime? date = opfDocument
                ?.Element(opfNamespace + "package")
                ?.Element(opfNamespace + "metadata")
                ?.Elements(dcNamespace + "date")
                ?.Where(d => {DateTime dateTime; return DateTime.TryParse(d.Value, out dateTime);})
                ?.Select(d => DateTime.Parse(d.Value, CultureInfo.InvariantCulture).ToUniversalTime())
                ?.OrderBy(d => d)
                ?.First();
            if (date is null)
            {
                return false;
            }
            metadata.Date = date;
            return true;
        }
    }
}