using System;
using System.Collections.Generic;
using System.Xml.Linq;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Services.EpubParsing
{
    public class XDocumentEpub2MetadataCreatorsParser : IXDocumentEpubMetadataFieldParser
    {
        private readonly CreatorsFormatter _formatter = new CreatorsFormatter();

        public bool TryParse(XDocument opfDocument, in EpubMetadata metadata)
        {
            XNamespace opfNamespace = EpubXmlNamespaceProvider.OpfNamespace;
            XNamespace dcNamespace = EpubXmlNamespaceProvider.DcNamespace;
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
                foreach (XElement creator in creators)
                {
                    string name = creator.Value;
                    string? role = creator.Attribute(opfNamespace + "role")?.Value;
                    ICollection<string>? roles;
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