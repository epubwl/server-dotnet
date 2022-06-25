using System.Xml.Linq;

namespace EpubWebLibraryServer.Areas.Library.Services
{
    public class XmlNamespaceProvider
    {
        public XNamespace ContainerNamespace => "urn:oasis:names:tc:opendocument:xmlns:container";

        public XNamespace DcNamespace => "http://purl.org/dc/elements/1.1/";

        public XNamespace OpfNamespace => "http://www.idpf.org/2007/opf";
    }
}