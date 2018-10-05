using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using SoapHttpClient.Enums;
using SoapHttpClient.Tests.Fixtures;
using SoapHttpClient.Tests.Fixtures.Attributes;
using Xunit;

namespace SoapHttpClient.Tests
{
    public class SoapClientTests
    {
        #region Customizations

        public class ActionIsNullCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customizations.Add(
                    new FilteringSpecimenBuilder(
                        new FixedBuilder(default(string)),
                        new ParameterSpecification(typeof(string), "action")));
            }
        }

        public class SoapVersion12Customization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customizations.Add(
                    new FilteringSpecimenBuilder(
                        new FixedBuilder(SoapVersion.Soap12),
                        new ParameterSpecification(typeof(SoapVersion), "soapVersion")));
            }
        }

        public class HeadersAreNullCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customizations.Add(
                    new FilteringSpecimenBuilder(
                        new FixedBuilder(default(IEnumerable<XElement>)),
                        new ParameterSpecification(typeof(IEnumerable<XElement>), "headers")));
            }
        }

        public class HeadersAreEmptyCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customizations.Add(
                    new FilteringSpecimenBuilder(
                        new FixedBuilder(Enumerable.Empty<XElement>()),
                        new ParameterSpecification(typeof(IEnumerable<XElement>), "headers")));
            }
        }

        public class OnlyOneHeaderAndOneBodyCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customizations.Add(
                    new FilteringSpecimenBuilder(
                        new FixedBuilder(new[] { fixture.Create<XElement>() }),
                        new ParameterSpecification(typeof(IEnumerable<XElement>), "headers")));

                fixture.Customizations.Add(
                    new FilteringSpecimenBuilder(
                        new FixedBuilder(new[] { fixture.Create<XElement>() }),
                        new ParameterSpecification(typeof(IEnumerable<XElement>), "bodies")));
            }
        }

        #endregion Customizations

        [Theory(DisplayName = "SoapClient should be assignable to the interface")]
        [DefaultData]
        public void Sut_ShouldBeAssignableTo_ISoapClient(SoapClient sut)
        {
            // Verify outcome
            sut.Should().BeAssignableTo<ISoapClient>();
        }

        [Theory(DisplayName = "Post should do expected HTTP call")]
        [InlineDefaultData(typeof(ActionIsNullCustomization))]
        [InlineDefaultData(typeof(SoapVersion12Customization))]
        [InlineDefaultData(typeof(HeadersAreNullCustomization))]
        [InlineDefaultData(typeof(HeadersAreEmptyCustomization))]
        [InlineDefaultData(typeof(OnlyOneHeaderAndOneBodyCustomization))]
        public async void PostAsync_ShouldDoExpectedHttpCall(
            Uri endpoint,
            string action,
            SoapVersion soapVersion,
            List<XElement> bodies,
            List<XElement> headers)
        {
            // Setup
            // -- We use TestMessageHandler in order to check what call does the inner HttpClient made
            var testMessageHandler = new TestMessageHandler();
            var httpClient = new HttpClient(testMessageHandler);
            var sut = new SoapClient(() => httpClient);

            // Exercise
            var actual = await sut.PostAsync(endpoint, soapVersion, bodies, headers, action);

            // Verify outcome
            // -- Assert that we only made one call
            actual.StatusCode.Should().Be(HttpStatusCode.OK);
            var actualCall = testMessageHandler.CallStack.Should().ContainSingle().Subject;
            // -- Assert the endpoint
            actualCall.Uri.Should().Be(endpoint);
            // -- Assert the headers
            AssertActualHeaders(actualCall.Headers, soapVersion, action);
            // -- Assert the request body
            AssertRequestBody(soapVersion, actualCall.Body, bodies, headers);
        }

        [Theory(DisplayName = "Post should cancel HTTP call")]
        [InlineDefaultData(typeof(ActionIsNullCustomization))]
        [InlineDefaultData(typeof(SoapVersion12Customization))]
        [InlineDefaultData(typeof(HeadersAreNullCustomization))]
        [InlineDefaultData(typeof(HeadersAreEmptyCustomization))]
        [InlineDefaultData(typeof(OnlyOneHeaderAndOneBodyCustomization))]
        public void PostAsync_ShouldCancelExpectedHttpCall(
            Uri endpoint,
            string action,
            SoapVersion soapVersion,
            List<XElement> bodies,
            List<XElement> headers)
        {
            // Setup
            // -- We use TestMessageHandler in order to check what call does the inner HttpClient made
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var testMessageHandler = new TestMessageHandler();
            var httpClient = new HttpClient(testMessageHandler);
            var sut = new SoapClient(() => httpClient);

            // Exercise
            var task = sut.PostAsync(endpoint, soapVersion, bodies, headers, action, token);

            // Verify outcome
            // -- Assert that we only made one call
            tokenSource.Cancel(true);
            Task.WaitAll(task);
            Assert.True(token.IsCancellationRequested);
            Console.WriteLine(task.Result);
        }

        #region Private Methods

        private static void AssertActualHeaders(HttpContentHeaders headers, SoapVersion soapVersion, string action)
        {
            if (headers.Contains("ActionHeader"))
            {
                soapVersion.Should().Be(SoapVersion.Soap11);
                headers.GetValues("ActionHeader").Single().Should().Be(action);
            }
            else if (headers.ContentType.Parameters.Any(e => e.Name == "ActionParameter"))
            {
                var actionParam = headers.ContentType.Parameters.Single(e => e.Name == "ActionParameter");
                actionParam.Value.Should().Be($"\"{action}\"");
                soapVersion.Should().Be(SoapVersion.Soap12);
            }
        }

        private static void AssertRequestBody(
            SoapVersion soapVersion,
            string body,
            IEnumerable<XElement> bodies,
            IReadOnlyCollection<XElement> headers)
        {
            // Get Namespaces
            var expectedNameSpace =
                (soapVersion == SoapVersion.Soap11)
                    ? "http://schemas.xmlsoap.org/soap/envelope/"
                    : "http://www.w3.org/2003/05/soap-envelope";

            var actualEnvelope = XElement.Parse(body);

            // Assert Envelope
            actualEnvelope.Name.LocalName.Should().Be("Envelope");
            actualEnvelope.Name.Namespace.NamespaceName.Should().Be(expectedNameSpace);
            
            // Assert Headers
            
            if (headers != null && headers.Any())
            {
                var actualHeader =
                    actualEnvelope.Elements()
                    .Where(e => e.Name.LocalName == "Header")
                    .Where(e => e.Name.Namespace.NamespaceName == expectedNameSpace)
                    .Should()
                    .ContainSingle()
                    .Subject;

                // Since FluentAssertions Should().Be(...) compares by instance and we dont have the same instance
                // And Should().BeEquivalentTo(...) compares by object graph and generates a memory degration
                // we will compare the elements using the .ToString() serialization of the XML
                actualHeader.Elements().Select(e => e.ToString())
                    .Should().BeEquivalentTo(headers.Select(e => e.ToString()));
            }

            // Assert Bodies
            var actualBody =
                actualEnvelope.Elements()
                .Where(e => e.Name.LocalName == "Body")
                .Where(e => e.Name.Namespace.NamespaceName == expectedNameSpace)
                .Should()
                .ContainSingle()
                .Subject;

            // Since FluentAssertions Should().Be(...) compares by instance and we dont have the same instance
            // And Should().BeEquivalentTo(...) compares by object graph and generates a memory degration
            // we will compare the elements using the .ToString() serialization of the XML
            actualBody.Elements().Select(e => e.ToString())
                .Should().BeEquivalentTo(bodies.Select(e => e.ToString()));
        }

        #endregion Private Methods
    }
}
