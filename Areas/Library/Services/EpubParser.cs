using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Services
{
    public class EpubParser
    {
        private readonly Epub3ParsingStrategy _epub3ParsingStrategy;

        private readonly Epub2ParsingStrategy _epub2ParsingStrategy;

        private readonly XmlNamespaceProvider _xmlNamespaceProvider;

        public EpubParser(Epub3ParsingStrategy epub3ParsingStrategy, Epub2ParsingStrategy epub2ParsingStrategy, XmlNamespaceProvider xmlNamespaceProvider)
        {
            _epub3ParsingStrategy = epub3ParsingStrategy;
            _epub2ParsingStrategy = epub2ParsingStrategy;
            _xmlNamespaceProvider = xmlNamespaceProvider;
        }

        public bool TryParse(Stream epubStream, in EpubMetadata metadata, out Stream coverStream, out string coverMimetype)
        {
            try
            {
                using (ZipArchive zipArchive = new ZipArchive(epubStream, ZipArchiveMode.Read, true))
                {
                    XDocument containerXmlDocument = GetContainerXmlDocument(zipArchive);
                    string opfPath = GetOpfPath(zipArchive, containerXmlDocument);
                    XDocument opfDocument = GetOpfDocument(zipArchive, opfPath);
                    IEpubParsingStrategy strategy = GetEpubParsingStrategy(opfDocument);
                    return new bool[]{
                        strategy.TryParseCover(zipArchive, opfDocument, opfPath, out coverStream, out coverMimetype),
                        strategy.TryParseDate(opfDocument, in metadata),
                        strategy.TryParseTitle(opfDocument, in metadata)
                    }.Any(b => b);
                }
                
            }
            catch (Exception)
            {
                coverStream = Stream.Null;
                coverMimetype = "application/octet-stream";
                return false;
            }
        }

        private XDocument GetContainerXmlDocument(ZipArchive zipArchive)
        {
            ZipArchiveEntry? containerXml = zipArchive.GetEntry("META-INF/container.xml");
            if (containerXml is null)
            {
                throw new FileNotFoundException();
            }
            using (Stream containerXmlStream = containerXml.Open())
            {
                XDocument? containerXmlDocument = XDocument.Load(containerXmlStream);
                if (containerXmlDocument is null)
                {
                    throw new FileNotFoundException();
                }
                return containerXmlDocument;
            }
        }

        private string GetOpfPath(ZipArchive zipArchive, XDocument containerXmlDocument)
        {
            string? opfPath = containerXmlDocument
                ?.Element(_xmlNamespaceProvider.ContainerNamespace + "container")
                ?.Element(_xmlNamespaceProvider.ContainerNamespace + "rootfiles")
                ?.Element(_xmlNamespaceProvider.ContainerNamespace + "rootfile")
                ?.Attribute("full-path")?.Value;
            if (opfPath is null)
            {
                throw new FileNotFoundException();
            }
            return opfPath;
        }

        private XDocument GetOpfDocument(ZipArchive zipArchive, string opfPath)
        {
            ZipArchiveEntry? opfFile = zipArchive.GetEntry(opfPath);
            if (opfFile is null)
            {
                throw new FileNotFoundException();
            }
            using(Stream opfStream = opfFile.Open())
            {
                XDocument? opfDocument = XDocument.Load(opfStream);
                if (opfDocument is null)
                {
                    throw new FileNotFoundException();
                }
                return opfDocument;
            }
        }

        private IEpubParsingStrategy GetEpubParsingStrategy(XDocument opfDocument)
        {
            string? version = opfDocument.Elements()
                .Where(e => e.Name.LocalName == "package").SingleOrDefault()
                ?.Attribute("version")?.Value;
            switch (version)
            {
                case "3.0":
                    return _epub3ParsingStrategy;
                case "2.0":
                    return _epub2ParsingStrategy;
                default:
                    throw new ArgumentException();
            }
        }
    }
}