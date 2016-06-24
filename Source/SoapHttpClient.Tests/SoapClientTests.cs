using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using Should;

namespace SoapHttpClient.Tests
{
    [TestFixture]
    public class SoapClientTests
    {
        private const string FakeEndpoint = "https://example.com/soap";
        private const string Soap11MediaType = "text/xml";
        private const string Soap12MediaType = "application/soap+xml";
        private const string SoapCharSet = "utf-8";

        private const string FakeResponseNoHeader =
@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soapenv:Body>
    <FakeMethod />
  </soapenv:Body>
</soapenv:Envelope>";

        private const string FakeAction = "https://example.com/soap/action";

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

        [Test]
        public void Soap11ClientIncludesSoapActionParameter()
        {
            var mockMessageHandler = new MockHttpMessageHandler();
            mockMessageHandler
                .Expect(HttpMethod.Post, FakeEndpoint)
                .With(req => req.Content.Headers.Single(h => h.Key == "SOAPAction").Value.Single() == FakeAction)
                .Respond(HttpStatusCode.OK);

            Task<HttpResponseMessage> result;
            using (var sut = new SoapClient(() => new HttpClient(mockMessageHandler)))
            {
                result = sut.PostAsync(FakeEndpoint, _fakeBody, action: FakeAction);
            }
            result.ShouldBeType(typeof(Task<HttpResponseMessage>));
            mockMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Test]
        public void Soap11ClientIssuesAValidRequest()
        {
            var mockMessageHandler = new MockHttpMessageHandler();
            mockMessageHandler
                .Expect(HttpMethod.Post, FakeEndpoint)
                .With(req => req.Content.Headers.ContentType.MediaType == Soap11MediaType)
                .With(req => req.Content.Headers.ContentType.CharSet == SoapCharSet)
                .Respond(HttpStatusCode.OK);

            Task<HttpResponseMessage> result;
            using (var sut = new SoapClient(() => new HttpClient(mockMessageHandler))) {
                result = sut.PostAsync(FakeEndpoint, _fakeBody);
            }
            result.ShouldBeType(typeof(Task<HttpResponseMessage>));
            mockMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Test]
        public void Soap12ClientIncludesSoapActionParameter()
        {
            var mockMessageHandler = new MockHttpMessageHandler();
            mockMessageHandler
                .Expect(HttpMethod.Post, FakeEndpoint)
                .With(
                    req =>
                        req.Content.Headers.ContentType.Parameters.Single(h => h.Name == "action").Value ==
                        $"\"{FakeAction}\"")
                .Respond(HttpStatusCode.OK);

            Task<HttpResponseMessage> result;
            using (var sut = new SoapClient(() => new HttpClient(mockMessageHandler), SoapVersion.Soap12))
            {
                result = sut.PostAsync(FakeEndpoint, _fakeBody, action: FakeAction);
            }
            result.ShouldBeType(typeof(Task<HttpResponseMessage>));
            mockMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Test]
        public void Soap12ClientIssuesAValidRequest()
        {
            var mockMessageHandler = new MockHttpMessageHandler();
            mockMessageHandler
                .Expect(HttpMethod.Post, FakeEndpoint)
                .With(req => req.Content.Headers.ContentType.MediaType == Soap12MediaType)
                .With(req => req.Content.Headers.ContentType.CharSet == SoapCharSet)
                .Respond(HttpStatusCode.OK);

            Task<HttpResponseMessage> result;
            using (var sut = new SoapClient(() => new HttpClient(mockMessageHandler), SoapVersion.Soap12))
            {
                result = sut.PostAsync(FakeEndpoint, _fakeBody);
            }
            result.ShouldBeType(typeof(Task<HttpResponseMessage>));
            mockMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Test]
        public void SoapClientIssuesAValidRequest()
        {
            var mockMessageHandler = new MockHttpMessageHandler();
            mockMessageHandler
                .Expect(HttpMethod.Post, FakeEndpoint)
                .With(req => req.Content.ReadAsStringAsync().Result == FakeResponseNoHeader)
                .Respond(HttpStatusCode.OK);

            Task<HttpResponseMessage> result;
            using (var sut = new SoapClient(() => new HttpClient(mockMessageHandler)))
            {
                result = sut.PostAsync(FakeEndpoint, _fakeBody);
            }
            result.ShouldBeType(typeof(Task<HttpResponseMessage>));
            mockMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Test]
        public void SoapClientIssuesAValidRequestWithHeader()
        {
            var mockMessageHandler = new MockHttpMessageHandler();
            mockMessageHandler
                .Expect(HttpMethod.Post, FakeEndpoint)
                .With(req => req.Content.ReadAsStringAsync().Result == FakeResponseWithHeader)
                .Respond(HttpStatusCode.OK);

            Task<HttpResponseMessage> result;
            using (var sut = new SoapClient(() => new HttpClient(mockMessageHandler)))
            {
                result = sut.PostAsync(FakeEndpoint, _fakeBody, _fakeHeader);
            }
            result.ShouldBeType(typeof(Task<HttpResponseMessage>));
            mockMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Test]
        public void SoapClientRequiresABodyToBeProvided() {
            var sut = new SoapClient();
            Action action = () => sut.PostAsync(FakeEndpoint, null).Wait();
            action.ShouldThrow<AggregateException>();
        }
    }
}