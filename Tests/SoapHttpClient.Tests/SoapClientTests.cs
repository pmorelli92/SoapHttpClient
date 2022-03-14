using System.Net;
using System.Xml.Linq;
using SoapHttpClient.DTO;
using SoapHttpClient.Enums;
using Xunit;

namespace SoapHttpClient.Tests;

public class SoapClientTests
{
    [Fact(DisplayName = "SoapClient should be assignable to the interface")]
    public void Sut_ShouldBeAssignableTo_ISoapClient()
    {
        // Verify outcome
        Assert.IsAssignableFrom<ISoapClient>(new SoapClient());
    }

    public static IEnumerable<object?[]> PostAsyncTestsData =>
        new List<object?[]>
        {
            // Action is null
            new object?[] {
                new Uri("https://test.com"),
                SoapVersion.Soap11,
                new[] { new XElement("body1"), new XElement("body2") },
                new[] { new XElement("header1") },
                null
            },

            // Soap 12
            new object?[] {
                new Uri("https://test.com"),
                SoapVersion.Soap12,
                new[] { new XElement("body1") },
                new[] { new XElement("header1"), new XElement("header2") },
                "action"
            },

            // Headers are null
            new object?[] {
                new Uri("https://test.com"),
                SoapVersion.Soap12,
                new[] { new XElement("body1") },
                null,
                "action"
            },

            // Headers are empty
            new object?[] {
                new Uri("https://test.com"),
                SoapVersion.Soap12,
                new[] { new XElement("body1") },
                new XElement[] {},
                "action"
            },

            // One header and one body
            new object?[] {
                new Uri("https://test.com"),
                SoapVersion.Soap11,
                new[] { new XElement("body1") },
                new[] { new XElement("header1") },
                "action"
            },
        };

    [Theory]
    [MemberData(nameof(PostAsyncTestsData))]
    public async void PostAsyncTests(
        Uri endpoint,
        SoapVersion version,
        IEnumerable<XElement> bodies,
        IEnumerable<XElement> headers,
        string action)
    {
        // Setup
        var testFactory = new TestHttpClientFactory();
        var sut = new SoapClient(testFactory);

        // Exercise
        var result = await sut.PostAsync(endpoint, version, bodies, headers, action);

        // Verify outcome
        var actual = testFactory.Handler.CallStack.Single();

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(endpoint, actual.Uri);

        // Assert action and version
        var actualHeaders = actual.Headers;
        if (actualHeaders.Contains("ActionHeader"))
        {
            var actualVersion = SoapVersion.Soap11;
            Assert.Equal(version, actualVersion);
            Assert.Equal(action, actualHeaders.GetValues("ActionHeader").Single());
        }
        else
        {
            var actionParam = actualHeaders.ContentType?.Parameters?.SingleOrDefault(e => e.Name == "ActionParameter");
            if (actionParam != null)
            {
                var actualVersion = SoapVersion.Soap12;
                Assert.Equal(version, actualVersion);
                Assert.Equal($"\"{action}\"", actionParam.Value);
            }
        }

        // Assert namespace
        var actualEnvelope = XElement.Parse(actual.Body);
        var expectedNamespace = new SoapMessageConfiguration(version).Schema;
        Assert.Equal("Envelope", actualEnvelope.Name.LocalName);
        Assert.Equal(expectedNamespace, actualEnvelope.Name.Namespace.NamespaceName);

        // Assert Headers
        if (headers != null && headers.Any())
        {
            var actualHeader =
                actualEnvelope.Elements()
                .Where(e => e.Name.LocalName == "Header")
                .Where(e => e.Name.Namespace.NamespaceName == expectedNamespace)
                .Single();

            Assert.Equal(
                headers.Select(e => e.ToString()),
                actualHeader.Elements().Select(e => e.ToString()));
        }

        // Assert Bodies
        var actualBody =
            actualEnvelope.Elements()
            .Where(e => e.Name.LocalName == "Body")
            .Where(e => e.Name.Namespace.NamespaceName == expectedNamespace)
            .Single();

        Assert.Equal(
            bodies.Select(e => e.ToString()),
            actualBody.Elements().Select(e => e.ToString()));
    }
}
