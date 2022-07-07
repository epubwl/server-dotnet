using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Services.EpubParsing
{
    public class ZipArchiveXDocumentEpubParser : IEpubParser
    {
        private readonly IZipArchiveXDocumentEpubMetadataCoverParser _epub2CoverParser = new ZipArchiveXDocumentEpub2MetadataCoverParser();

        private readonly IZipArchiveXDocumentEpubMetadataCoverParser _epub3CoverParser = new ZipArchiveXDocumentEpub3MetadataCoverParser();

        private readonly IReadOnlyCollection<IXDocumentEpubMetadataFieldParser> _epub2FieldParsers = new List<IXDocumentEpubMetadataFieldParser>{
                new XDocumentEpubMetadataTitleParser(),
                new XDocumentEpubMetadataDateParser(),
                new XDocumentEpub2MetadataCreatorsParser()
            }.AsReadOnly();
        
        private readonly IReadOnlyCollection<IXDocumentEpubMetadataFieldParser> _epub3FieldParsers = new List<IXDocumentEpubMetadataFieldParser>{
                new XDocumentEpubMetadataTitleParser(),
                new XDocumentEpubMetadataDateParser(),
                new XDocumentEpub3MetadataCreatorsParser()
            }.AsReadOnly();

        public bool TryParse(Stream epubStream, in EpubMetadata metadata, out Stream coverStream, out string coverMimetype)
        {
            try
            {
                using (ZipArchive zipArchive = new ZipArchive(epubStream, ZipArchiveMode.Read, true))
                {
                    XDocument containerXmlDocument = GetContainerXmlDocument(zipArchive);
                    string opfPath = GetOpfPath(zipArchive, containerXmlDocument);
                    XDocument opfDocument = GetOpfDocument(zipArchive, opfPath);
                    return SelectParserTryParse(zipArchive, opfDocument, opfPath, in metadata, out coverStream, out coverMimetype);
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
            XNamespace containerNamespace = EpubXmlNamespaceProvider.ContainerNamespace;
            string? opfPath = containerXmlDocument
                ?.Element(containerNamespace + "container")
                ?.Element(containerNamespace + "rootfiles")
                ?.Element(containerNamespace + "rootfile")
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

        private bool SelectParserTryParse(ZipArchive zipArchive, XDocument opfDocument, string opfPath, in EpubMetadata metadata, out Stream coverStream, out string coverMimetype)
        {
            IZipArchiveXDocumentEpubMetadataCoverParser coverParser;
            IReadOnlyCollection<IXDocumentEpubMetadataFieldParser> fieldParsers;
            string? version = opfDocument.Elements()
                .Where(e => e.Name.LocalName == "package").SingleOrDefault()
                ?.Attribute("version")?.Value;
            switch (version)
            {
                case "3.0":
                    coverParser = _epub3CoverParser;
                    fieldParsers = _epub3FieldParsers;
                    break;
                case "2.0":
                    coverParser = _epub2CoverParser;
                    fieldParsers = _epub2FieldParsers;
                    break;
                default:
                    throw new ArgumentException();
            }
            bool didParseCover = coverParser.TryParse(zipArchive, opfDocument, opfPath, out coverStream, out coverMimetype);
            var didParseFields = new List<bool>();
            foreach (IXDocumentEpubMetadataFieldParser fieldParser in fieldParsers)
            {
                didParseFields.Add(fieldParser.TryParse(opfDocument, in metadata));
            }
            return didParseCover || didParseFields.Any(b => b);
        }
    }
}