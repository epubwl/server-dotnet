using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace EpubWebLibraryServer.Areas.Library.Services.EpubParsing
{
    public class ZipArchiveXDocumentEpub3MetadataCoverParser : IZipArchiveXDocumentEpubMetadataCoverParser
    {
        public bool TryParse(ZipArchive zipArchive, XDocument opfDocument, string opfPath, out Stream coverStream, out string coverMimetype)
        {
            XNamespace opfNamespace = EpubXmlNamespaces.Opf;
            XElement? coverElement = opfDocument
                ?.Element(opfNamespace + "package")
                ?.Element(opfNamespace + "manifest")
                ?.Elements(opfNamespace + "item")
                ?.Where(e => e.Attribute("properties")?.Value == "cover-image")
                ?.First();
            string? href = coverElement?.Attribute("href")?.Value;
            string? coverPath = href is null
                ? null
                : Path.Join(Directory.GetParent(opfPath)?.Name, href).Replace("\\", "/");
            string? mediaType = coverElement?.Attribute("media-type")?.Value;
            try
            {
                coverStream = coverPath is null
                    ? Stream.Null
                    : zipArchive.GetEntry(coverPath)?.Open() ?? Stream.Null;
                coverMimetype = mediaType ?? EpubMimeTypes.Application.OctetStream;
                return true;
            }
            catch (Exception)
            {
                coverStream = Stream.Null;
                coverMimetype = EpubMimeTypes.Application.OctetStream;
                return false;
            }
        }
    }
}