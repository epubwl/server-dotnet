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
                ?.Select(d => ParseDateTime(d.Value))
                ?.Where(d => d is not null)
                ?.OrderBy(d => d)
                ?.FirstOrDefault();
            if (date is null)
            {
                return false;
            }
            metadata.Date = date;
            return true;
        }

        private DateTime? ParseDateTime(string? s)
        {
            return DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime dateTime)
                ? dateTime.ToUniversalTime()
                : (DateTime?) null;
        }
    }
}