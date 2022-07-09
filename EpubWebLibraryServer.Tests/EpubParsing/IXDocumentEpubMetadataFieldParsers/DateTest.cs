using System;
using System.Xml.Linq;
using EpubWebLibraryServer.Areas.Library.Services.EpubParsing;

namespace EpubWebLibraryServer.Tests.EpubParsing.IXDocumentEpubMetadataFieldParsers;

[TestClass]
public class Date
{
    private readonly Tester<DateTime> _tester = new Tester<DateTime>(
            new XDocumentEpubMetadataDateParser(),
            "Date"
        );

    [TestMethod]
    public void TestSingleDateUtc()
    {
        XNamespace opfNamespace = EpubXmlNamespaces.Opf;
        XNamespace dcNamespace = EpubXmlNamespaces.Dc;
        XDocument opfDocument = new XDocument(
            new XElement(opfNamespace + "package",
                new XElement(opfNamespace + "metadata",
                    new XElement(dcNamespace + "date", "2000-01-01T00:00:00Z")
                )
            )
        );
        DateTime expectedValue = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        _tester.TestTryParseSuccess(opfDocument, expectedValue);
    }

    [TestMethod]
    public void TestMultipleDatesUtc()
    {
        XNamespace opfNamespace = EpubXmlNamespaces.Opf;
        XNamespace dcNamespace = EpubXmlNamespaces.Dc;
        XDocument opfDocument = new XDocument(
            new XElement(opfNamespace + "package",
                new XElement(opfNamespace + "metadata",
                    new XElement(dcNamespace + "date", "2000-02-02T00:00:00Z"),
                    new XElement(dcNamespace + "date", "2000-01-01T00:00:00Z"),
                    new XElement(dcNamespace + "date", "2000-03-03T00:00:00Z")
                )
            )
        );
        DateTime expectedValue = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        _tester.TestTryParseSuccess(opfDocument, expectedValue);
    }

    [TestMethod]
    public void TestSingleDateNoTimezone()
    {
        XNamespace opfNamespace = EpubXmlNamespaces.Opf;
        XNamespace dcNamespace = EpubXmlNamespaces.Dc;
        XDocument opfDocument = new XDocument(
            new XElement(opfNamespace + "package",
                new XElement(opfNamespace + "metadata",
                    new XElement(dcNamespace + "date", "2000-01-01")
                )
            )
        );
        DateTime expectedValue = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        _tester.TestTryParseSuccess(opfDocument, expectedValue);
    }

    [TestMethod]
    public void TestMultipleDatesNoTimezone()
    {
        XNamespace opfNamespace = EpubXmlNamespaces.Opf;
        XNamespace dcNamespace = EpubXmlNamespaces.Dc;
        XDocument opfDocument = new XDocument(
            new XElement(opfNamespace + "package",
                new XElement(opfNamespace + "metadata",
                    new XElement(dcNamespace + "date", "2000-02-02"),
                    new XElement(dcNamespace + "date", "2000-01-01"),
                    new XElement(dcNamespace + "date", "2000-03-03")
                )
            )
        );
        DateTime expectedValue = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        _tester.TestTryParseSuccess(opfDocument, expectedValue);
    }

    [TestMethod]
    public void TestBlankDocument()
    {
        XDocument opfDocument = new XDocument();
        _tester.TestTryParseFail(opfDocument);
    }

    [TestMethod]
    public void TestInvalidDate()
    {
        XNamespace opfNamespace = EpubXmlNamespaces.Opf;
        XNamespace dcNamespace = EpubXmlNamespaces.Dc;
        XDocument opfDocument = new XDocument(
            new XElement(opfNamespace + "package",
                new XElement(opfNamespace + "metadata",
                    new XElement(dcNamespace + "date", "not a date")
                )
            )
        );
        _tester.TestTryParseFail(opfDocument);
    }
}