using System;
using System.Net.Http;
using System.Xml.Linq;
using Moq;
using NUnit.Framework;
using Should;
using SoapHttpClient.Extensions;
using SoapHttpClient.Interfaces;

namespace SoapHttpClient.Tests
{
    [TestFixture]
    public class SoapClientExtensionsTests
    {
        private const string FakeEndpoint = "https://example.com/soap";
        private const string FakeAction = "https://example.com/soap/action";

        private readonly XElement _fakeBody = new XElement(XName.Get("FakeMethod"));
        private readonly XElement _fakeHeader = new XElement(XName.Get("FakeHeader"));

        private static Mock<ISoapClient> GetClientMock()
        {
            var clientMock = new Mock<ISoapClient>();
            clientMock
                .Setup(x => x.PostAsync(
                    It.Is<string>(endpoint => endpoint == FakeEndpoint),
                    It.Is<XElement>(body => body.Name == "FakeMethod"),
                    It.Is<XElement>(header => header.Name == "FakeHeader"),
                    It.Is<string>(action => action == FakeAction)))
                .ReturnsAsync(new HttpResponseMessage())
                .Verifiable();

            return clientMock;
        }

        [Test]
        public void CallToPostAsyncWithUriEndpoint()
        {
            var clientMock = GetClientMock();
            clientMock.Object.PostAsync(new Uri(FakeEndpoint), _fakeBody, _fakeHeader, FakeAction).Wait();
            clientMock.VerifyAll();
        }

        [Test]
        public void CallToPostAsyncWithUriEndpointAndNullBody()
        {
            var clientMock = new Mock<ISoapClient>();
            Action action =
                () => clientMock.Object.PostAsync(new Uri(FakeEndpoint), null, null, null, FakeAction).Wait();
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void CallToPostAsyncWithUriEndpointAndNullSerializer()
        {
            var clientMock = GetClientMock();
            clientMock.Object.PostAsync(new Uri(FakeEndpoint), new FakeMethod(), new FakeHeader(), null, FakeAction)
                .Wait();
            clientMock.VerifyAll();
        }

        [Test]
        public void CallToPostAsyncWithUriEndpointAndSerializer()
        {
            var serializerMock = new Mock<IXElementSerializer>();
            serializerMock
                .Setup(ser => ser.Serialize(It.Is<string>(x => x == "FakeMethod")))
                .Returns(new XElement("FakeMethod"))
                .Verifiable();
            serializerMock
                .Setup(ser => ser.Serialize(It.Is<string>(x => x == "FakeHeader")))
                .Returns(new XElement("FakeHeader"))
                .Verifiable();
            var clientMock = GetClientMock();
            clientMock.Object.PostAsync(new Uri(FakeEndpoint), "FakeMethod", "FakeHeader", () => serializerMock.Object,
                FakeAction).Wait();
            clientMock.VerifyAll();
            serializerMock.VerifyAll();
        }

        [Test]
        public void CallToPostWithStringEndpoint()
        {
            var clientMock = GetClientMock();
            clientMock.Object.Post(FakeEndpoint, _fakeBody, _fakeHeader, FakeAction);
            clientMock.VerifyAll();
        }

        [Test]
        public void CallToPostWithUriEndpoint()
        {
            var clientMock = GetClientMock();
            clientMock.Object.Post(new Uri(FakeEndpoint), _fakeBody, _fakeHeader, FakeAction);
            clientMock.VerifyAll();
        }

        [Test]
        public void CallToPostWithUriEndpointAndNullSerializer()
        {
            var clientMock = GetClientMock();
            clientMock.Object.Post(new Uri(FakeEndpoint), new FakeMethod(), new FakeHeader(), null, FakeAction);
            clientMock.VerifyAll();
        }

        [Test]
        public void CallToPostWithUriEndpointAndSerializer()
        {
            var serializerMock = new Mock<IXElementSerializer>();
            serializerMock
                .Setup(ser => ser.Serialize(It.Is<string>(x => x == "FakeMethod")))
                .Returns(new XElement("FakeMethod"))
                .Verifiable();
            serializerMock
                .Setup(ser => ser.Serialize(It.Is<string>(x => x == "FakeHeader")))
                .Returns(new XElement("FakeHeader"))
                .Verifiable();
            var clientMock = GetClientMock();
            clientMock.Object.Post(new Uri(FakeEndpoint), "FakeMethod", "FakeHeader", () => serializerMock.Object,
                FakeAction);
            clientMock.VerifyAll();
            serializerMock.VerifyAll();
        }
    }

    public class FakeMethod
    {
    }

    public class FakeHeader
    {
    }
}