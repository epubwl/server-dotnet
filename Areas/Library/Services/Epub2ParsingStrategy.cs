using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Services
{
    public class Epub2ParsingStrategy : IEpubParsingStrategy
    {
        private readonly XmlNamespaceProvider _xmlNamespaceProvider;

        public Epub2ParsingStrategy(XmlNamespaceProvider xmlNamespaceProvider)
        {
            _xmlNamespaceProvider = xmlNamespaceProvider;
        }

        public bool TryParseCover(ZipArchive zipArchive, XDocument opfDocument, string opfPath, out Stream coverStream, out string coverMimetype)
        {
            string? coverId = opfDocument
                ?.Element(_xmlNamespaceProvider.OpfNamespace + "package")
                ?.Element(_xmlNamespaceProvider.OpfNamespace + "metadata")
                ?.Elements(_xmlNamespaceProvider.OpfNamespace + "meta")
                ?.Where(e => e.Attribute("name")?.Value == "cover")
                ?.First()?.Attribute("content")?.Value;
            XElement? coverElement = coverId is not null
                ? opfDocument
                    ?.Element(_xmlNamespaceProvider.OpfNamespace + "package")
                    ?.Element(_xmlNamespaceProvider.OpfNamespace + "manifest")
                    ?.Elements(_xmlNamespaceProvider.OpfNamespace + "item")
                    ?.Where(e => e.Attribute("id")?.Value == coverId)
                    ?.First()
                : null;
            string? href = coverElement?.Attribute("href")?.Value;
            string? coverPath = href is null
                ? null
                : Path.Join(Directory.GetParent(opfPath)?.Name, href);
            string? mediaType = coverElement?.Attribute("media-type")?.Value;
            try
            {
                coverStream = (coverId is null || coverPath is null)
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

        public bool TryParseCreators(XDocument opfDocument, in EpubMetadata metadata)
        {
            IEnumerable<XElement>? creators = opfDocument
                ?.Element(_xmlNamespaceProvider.OpfNamespace + "package")
                ?.Element(_xmlNamespaceProvider.OpfNamespace + "metadata")
                ?.Elements(_xmlNamespaceProvider.DcNamespace + "creator");
            if (creators is null)
            {
                return false;
            }
            try
            {
                var creatorRoles = new Dictionary<string, List<string>>();
                foreach (XElement creator in creators)
                {
                    string name = creator.Value;
                    string? role = creator.Attribute(_xmlNamespaceProvider.OpfNamespace + "role")?.Value;
                    List<string>? roles;
                    creatorRoles.TryGetValue(name, out roles);
                    if (roles is null)
                    {
                        roles = new List<string>();
                        creatorRoles[name] = roles;
                    }
                    if (role is not null)
                    {
                        roles.Add(role);
                    }
                }
                metadata.Creators = String.Join(", ",
                    creatorRoles.Select(kvp => {
                        return kvp.Value.Count > 0
                            ? $"{kvp.Key} ({String.Join(",", kvp.Value)})"
                            : kvp.Key;}));
                return true;
            }
            catch (Exception)
            {
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