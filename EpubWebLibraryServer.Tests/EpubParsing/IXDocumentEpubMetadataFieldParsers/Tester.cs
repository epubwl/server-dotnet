using System.Xml.Linq;
using EpubWebLibraryServer.Areas.Library.Models;
using EpubWebLibraryServer.Areas.Library.Services.EpubParsing;

namespace EpubWebLibraryServer.Tests.EpubParsing.IXDocumentEpubMetadataFieldParsers;

public class Tester<T>
{
    private readonly IXDocumentEpubMetadataFieldParser _fieldParser;

    private readonly string _propertyName;

    public Tester(IXDocumentEpubMetadataFieldParser fieldParser, string propertyName)
    {
        _fieldParser = fieldParser;
        _propertyName = propertyName;
    }

    private EpubMetadata GetBlankMetadata()
    {
        return new EpubMetadata();
    }

    public void TestTryParseSuccess(XDocument opfDocument, T expectedValue)
    {
        EpubMetadata blankMetadata = GetBlankMetadata();
        bool success = _fieldParser.TryParse(opfDocument, in blankMetadata);
        object? actualValue = blankMetadata.GetType().GetProperty(_propertyName)?.GetValue(blankMetadata);
        Assert.IsTrue(success);
        Assert.AreEqual(expectedValue, actualValue);
    }

    public void TestTryParseFail(XDocument opfDocument)
    {
        EpubMetadata blankMetadata = GetBlankMetadata();
        bool success = _fieldParser.TryParse(opfDocument, in blankMetadata);
        object? actualValue = blankMetadata.GetType().GetProperty(_propertyName)?.GetValue(blankMetadata);
        Assert.IsFalse(success);
        Assert.IsNull(actualValue);
    }
}