using FluentAssertions;
using RichardSzalay.MockHttp;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace SoapHttpClient.Tests
{
    public class SoapClientTests
    {
        private const string SoapCharSet = "utf-8";
        private const string Soap11MediaType = "text/xml";
        private const string Soap12MediaType = "application/soap+xml";
        private const string FakeEndpoint = "https://example.com/soap";
        private const string FakeAction = "https://example.com/soap/action";

        private const string FakeResponseNoHeader =
            @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"">
                <soapenv:Body>
                    <FakeMethod />
                </soapenv:Body>
            </soapenv:Envelope>";

        private const string FakeResponseWithHeader =
            @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"">
                <soapenv:Header>
                    <FakeHeader />
                </soapenv:Header>
                <soapenv:Body>
                    <FakeMethod />
                </soapenv:Body>
            </soapenv:Envelope>";

        private readonly XElement _fakeBody = new XElement(XName.Get("FakeMethod"));
        private readonly XElement _fakeHeader = new XElement(XName.Get("FakeHeader"));

        [Fact]
        public void Soap11ClientIncludesSoapActionParameter()
        {
            // Setup
            Task<HttpResponseMessage> result;
            var mockMessageHandler = new MockHttpMessageHandler();

            mockMessageHandler
                .Expect(HttpMethod.Post, FakeEndpoint)
                .With(req => req.Content.Headers.Single(h => h.Key == "SOAPAction").Value.Single() == FakeAction)
                .Respond(HttpStatusCode.OK);

            // Exercise
            using (var sut = new SoapClient(() => new HttpClient(mockMessageHandler)))
            {
                result = sut.PostAsync(FakeEndpoint, _fakeBody, action: FakeAction);
            }

            // Verify outcome
            VerifyHandlerWasUsedAndResponseType(mockMessageHandler, result);
        }

        [Fact]
        public void Soap11ClientIssuesAValidRequest()
        {
            // Setup
            Task<HttpResponseMessage> result;
            var mockMessageHandler = new MockHttpMessageHandler();

            mockMessageHandler
                .Expect(HttpMethod.Post, FakeEndpoint)
                .With(req => req.Content.Headers.ContentType.MediaType == Soap11MediaType)
                .With(req => req.Content.Headers.ContentType.CharSet == SoapCharSet)
                .Respond(HttpStatusCode.OK);

            // Exercise
            using (var sut = new SoapClient(() => new HttpClient(mockMessageHandler)))
            {
                result = sut.PostAsync(FakeEndpoint, _fakeBody);
            }

            // Verify outcome
            VerifyHandlerWasUsedAndResponseType(mockMessageHandler, result);
        }

        [Fact]
        public void Soap12ClientIncludesSoapActionParameter()
        {
            // Setup
            Task<HttpResponseMessage> result;
            var mockMessageHandler = new MockHttpMessageHandler();

            mockMessageHandler
                .Expect(HttpMethod.Post, FakeEndpoint)
                .With(
                    req =>
                        req.Content.Headers.ContentType.Parameters.Single(h => h.Name == "action").Value ==
                        $"\"{FakeAction}\"")
                .Respond(HttpStatusCode.OK);

            // Exercise
            using (var sut = new SoapClient(() => new HttpClient(mockMessageHandler), SoapVersion.Soap12))
            {
                result = sut.PostAsync(FakeEndpoint, _fakeBody, action: FakeAction);
            }

            // Verify outcome
            VerifyHandlerWasUsedAndResponseType(mockMessageHandler, result);
        }

        [Fact]
        public void Soap12ClientIssuesAValidRequest()
        {
            // Setup
            Task<HttpResponseMessage> result;
            var mockMessageHandler = new MockHttpMessageHandler();

            mockMessageHandler
                .Expect(HttpMethod.Post, FakeEndpoint)
                .With(req => req.Content.Headers.ContentType.MediaType == Soap12MediaType)
                .With(req => req.Content.Headers.ContentType.CharSet == SoapCharSet)
                .Respond(HttpStatusCode.OK);

            // Exercise
            using (var sut = new SoapClient(() => new HttpClient(mockMessageHandler), SoapVersion.Soap12))
            {
                result = sut.PostAsync(FakeEndpoint, _fakeBody);
            }

            // Verify outcome
            VerifyHandlerWasUsedAndResponseType(mockMessageHandler, result);
        }

        [Fact]
        public void SoapClientIssuesAValidRequest()
        {
            // Setup
            Task<HttpResponseMessage> result;
            var mockMessageHandler = new MockHttpMessageHandler();

            mockMessageHandler
                .Expect(HttpMethod.Post, FakeEndpoint)
                .Respond(HttpStatusCode.OK);

            // Exercise
            using (var sut = new SoapClient(() => new HttpClient(mockMessageHandler)))
            {
                result = sut.PostAsync(FakeEndpoint, _fakeBody);
                result.Wait();
            }

            // Verify outcome
            VerifyHandlerWasUsedAndResponseType(mockMessageHandler, result);
        }

        [Fact]
        public void SoapClientIssuesAValidRequestWithHeader()
        {
            // Setup
            Task<HttpResponseMessage> result;
            var mockMessageHandler = new MockHttpMessageHandler();

            mockMessageHandler
                .Expect(HttpMethod.Post, FakeEndpoint)
                .Respond(HttpStatusCode.OK);

            // Exercise
            using (var sut = new SoapClient(() => new HttpClient(mockMessageHandler)))
            {
                result = sut.PostAsync(FakeEndpoint, _fakeBody, _fakeHeader);
            }

            // Verify outcome
            VerifyHandlerWasUsedAndResponseType(mockMessageHandler, result);
        }

        [Fact]
        public void SoapClientRequiresABodyToBeProvided()
        {
            // Setup
            var sut = new SoapClient();

            // Exercise
            Action act = () => sut.PostAsync(FakeEndpoint, null).Wait();

            // Verify outcome
            act.ShouldThrowExactly<ArgumentNullException>();
        }

        #region Private Methods

        private void VerifyHandlerWasUsedAndResponseType(
            MockHttpMessageHandler mockMessageHandler,
            Task<HttpResponseMessage> result)
        {
            mockMessageHandler.VerifyNoOutstandingExpectation();
            result.Should().BeOfType(typeof(Task<HttpResponseMessage>));
        }

        #endregion Private Methods
    }
}