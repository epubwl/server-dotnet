using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Services
{
    public class Epub3ParsingStrategy : IEpubParsingStrategy
    {
        private readonly XmlNamespaceProvider _xmlNamespaceProvider;

        public Epub3ParsingStrategy(XmlNamespaceProvider xmlNamespaceProvider)
        {
            _xmlNamespaceProvider = xmlNamespaceProvider;
        }

        public bool TryParseCover(ZipArchive zipArchive, XDocument opfDocument, string opfPath, out Stream coverStream, out string coverMimetype)
        {
            XElement? coverElement = opfDocument
                ?.Element(_xmlNamespaceProvider.OpfNamespace + "package")
                ?.Element(_xmlNamespaceProvider.OpfNamespace + "manifest")
                ?.Elements(_xmlNamespaceProvider.OpfNamespace + "item")
                ?.Where(e => e.Attribute("properties")?.Value == "cover-image")
                ?.First();
            string? href = coverElement?.Attribute("href")?.Value;
            string? coverPath = href is null
                ? null
                : Path.Join(Directory.GetParent(opfPath)?.Name, href);
            string? mediaType = coverElement?.Attribute("media-type")?.Value;
            try
            {
                coverStream = coverPath is null
                    ? Stream.Null
                    : zipArchive.GetEntry(coverPath.Replace("\\", "/"))?.Open() ?? Stream.Null;
                coverMimetype = mediaType ?? "application/octet-stream";
                return true;
            }
            catch (Exception)
            {
                coverStream = Stream.Null;
                coverMimetype = "application/octet-stream";
                return false;
            }
        }

        public bool TryParseDate(XDocument opfDocument, in EpubMetadata metadata)
        {
            string? date = opfDocument
                ?.Element(_xmlNamespaceProvider.OpfNamespace + "package")
                ?.Element(_xmlNamespaceProvider.OpfNamespace + "metadata")
                ?.Element(_xmlNamespaceProvider.DcNamespace + "date")
                ?.Value;
            if (date is not null)
            {
                try
                {
                    metadata.Date = DateTime.Parse(date, CultureInfo.InvariantCulture);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        public bool TryParseTitle(XDocument opfDocument, in EpubMetadata metadata)
        {
            string? title = opfDocument
                ?.Element(_xmlNamespaceProvider.OpfNamespace + "package")
                ?.Element(_xmlNamespaceProvider.OpfNamespace + "metadata")
                ?.Element(_xmlNamespaceProvider.DcNamespace + "title")
                ?.Value;
            if (title is not null)
            {
                metadata.Title = title;
                return true;
            }
            return false;
        }
    }
}