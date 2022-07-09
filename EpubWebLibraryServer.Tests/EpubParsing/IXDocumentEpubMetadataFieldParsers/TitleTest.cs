using System.Xml.Linq;
using EpubWebLibraryServer.Areas.Library.Services.EpubParsing;

namespace EpubWebLibraryServer.Tests.EpubParsing.IXDocumentEpubMetadataFieldParsers;

[TestClass]
public class TitleTest
{
    private readonly Tester<string> _tester = new Tester<string>(
            new XDocumentEpubMetadataTitleParser(),
            "Title"
        );

    [TestMethod]
    public void TestSingleTitle()
    {
        XNamespace opfNamespace = EpubXmlNamespaces.Opf;
        XNamespace dcNamespace = EpubXmlNamespaces.Dc;
        XDocument opfDocument = new XDocument(
            new XElement(opfNamespace + "package",
                new XElement(opfNamespace + "metadata",
                    new XElement(dcNamespace + "title", "Only Title")
                )
            )
        );
        string expectedValue = "Only Title";
        _tester.TestTryParseSuccess(opfDocument, expectedValue);
    }

    [TestMethod]
    public void TestMultipleTitles()
    {
        XNamespace opfNamespace = EpubXmlNamespaces.Opf;
        XNamespace dcNamespace = EpubXmlNamespaces.Dc;
        XDocument opfDocument = new XDocument(
            new XElement(opfNamespace + "package",
                new XElement(opfNamespace + "metadata",
                    new XElement(dcNamespace + "title", "First Title"),
                    new XElement(dcNamespace + "title", "Second Title"),
                    new XElement(dcNamespace + "title", "Third Title")
                )
            )
        );
        string expectedValue = "First Title";
        _tester.TestTryParseSuccess(opfDocument, expectedValue);
    }

    [TestMethod]
    public void TestBlankDocument()
    {
        XDocument opfDocument = new XDocument();
        _tester.TestTryParseFail(opfDocument);
    }
}