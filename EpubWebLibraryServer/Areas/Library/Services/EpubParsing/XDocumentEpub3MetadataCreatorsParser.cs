using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Services.EpubParsing
{
    public class XDocumentEpub3MetadataCreatorsParser : IXDocumentEpubMetadataFieldParser
    {
        private readonly CreatorsFormatter _formatter = new CreatorsFormatter();

        public bool TryParse(XDocument opfDocument, in EpubMetadata metadata)
        {
            XNamespace opfNamespace = EpubXmlNamespaces.Opf;
            XNamespace dcNamespace = EpubXmlNamespaces.Dc;
            IEnumerable<XElement>? creators = opfDocument
                ?.Element(opfNamespace + "package")
                ?.Element(opfNamespace + "metadata")
                ?.Elements(dcNamespace + "creator");
            if (creators is null)
            {
                return false;
            }
            try
            {
                var creatorRoles = new Dictionary<string, ICollection<string>>();
                var creatorIds = new Dictionary<string, ICollection<string>>();
                foreach (XElement creator in creators)
                {
                    string name = creator.Value;
                    string? id = creator.Attribute("id")?.Value;
                    if (id is not null)
                    {
                        IEnumerable<XElement>? metaRoles = opfDocument
                            ?.Element(opfNamespace + "package")
                            ?.Element(opfNamespace + "metadata")
                            ?.Elements(dcNamespace + "meta")
                            .Where(e => e.Attribute("refines")?.Value == $"#{id}");
                        ICollection<string>? roles;
                        creatorRoles.TryGetValue(name, out roles);
                        if (roles is null)
                        {
                            roles = new List<string>();
                            creatorRoles[name] = roles;
                        }
                        if (metaRoles is not null)
                        {
                            foreach (XElement metaRole in metaRoles)
                            {
                                if (!String.IsNullOrEmpty(metaRole.Value))
                                {
                                    roles.Add(metaRole.Value);
                                }
                            }
                        }
                    }
                }
                metadata.Creators = _formatter.Format(creatorRoles);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}