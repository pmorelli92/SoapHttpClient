using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using Xunit;

namespace SoapHttpClient.Tests
{
    public class SoapClientExtensionsTests
    {
        private const string FakeEndpoint = "https://example.com/soap";
        private const string FakeAction = "https://example.com/soap/action";

        private readonly XElement _fakeBody = new XElement(XName.Get("FakeMethod"));
        private readonly XElement _fakeHeader = new XElement(XName.Get("FakeHeader"));

        private readonly IEnumerable<XElement> _fakeBodyElements = new []
        {
            new XElement("BodyElement1"),
            new XElement("BodyElement2")
        };
        private readonly IEnumerable<XElement> _fakeHeaderElements = new[]
        {
            new XElement("HeaderElement1"),
            new XElement("HeaderElement2")
        };

        [Fact]
        public void CallToPostAsyncWithUriEndpoint()
        {
            // Setup
            var clientMock = GetClientMock();

            // Exercise
            clientMock.Object.PostAsync(new Uri(FakeEndpoint), _fakeBody, _fakeHeader, FakeAction).Wait();

            // Verify outcome
            clientMock.VerifyAll();
        }

        [Fact]
        public void CallToPostAsyncWithUriEndpointMultiple()
        {
            // Setup
            var clientMock = GetClientMockMultiple();

            // Exercise
            clientMock.Object.PostAsync(new Uri(FakeEndpoint), _fakeBodyElements, _fakeHeaderElements, FakeAction).Wait();

            // Verify outcome
            clientMock.VerifyAll();
        }

        [Fact]
        public void CallToPostAsyncWithUriEndpointAndNullBody()
        {
            // Setup
            var clientMock = new Mock<ISoapClient>();

            // Exercise
            Action act = () => clientMock.Object.PostAsync(new Uri(FakeEndpoint), null, null, null, FakeAction).Wait();

            // Verify outcome
            act.ShouldThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void CallToPostAsyncWithUriEndpointAndNullSerializer()
        {
            // Setup
            var clientMock = GetClientMock();

            // Exercise
            clientMock.Object.PostAsync(new Uri(FakeEndpoint), new FakeMethod(), new FakeHeader(), null, FakeAction).Wait();

            // Verify outcome
            clientMock.VerifyAll();
        }

        [Fact]
        public void CallToPostAsyncWithUriEndpointAndSerializer()
        {
            // Setup
            var clientMock = GetClientMock();
            var serializerMock = new Mock<IXElementSerializer>();

            serializerMock
                .Setup(ser => ser.Serialize(It.Is<string>(x => x == "FakeMethod")))
                .Returns(new XElement("FakeMethod"))
                .Verifiable();

            serializerMock
                .Setup(ser => ser.Serialize(It.Is<string>(x => x == "FakeHeader")))
                .Returns(new XElement("FakeHeader"))
                .Verifiable();

            // Exercise
            clientMock.Object.PostAsync(new Uri(FakeEndpoint), "FakeMethod", "FakeHeader", () => serializerMock.Object, FakeAction).Wait();
            
            // Verify outcome
            clientMock.VerifyAll();
            serializerMock.VerifyAll();
        }

        [Fact]
        public void CallToPostWithStringEndpoint()
        {
            // Setup
            var clientMock = GetClientMock();

            // Exercise
            clientMock.Object.Post(FakeEndpoint, _fakeBody, _fakeHeader, FakeAction);

            // Verify outcome
            clientMock.VerifyAll();
        }

        [Fact]
        public void CallToPostWithUriEndpoint()
        {
            // Setup
            var clientMock = GetClientMock();

            // Exercise
            clientMock.Object.Post(new Uri(FakeEndpoint), _fakeBody, _fakeHeader, FakeAction);

            // Verify outcome
            clientMock.VerifyAll();
        }

        [Fact]
        public void CallToPostWithUriEndpointAndNullSerializer()
        {
            // Setup
            var clientMock = GetClientMock();

            // Exercise
            clientMock.Object.Post(new Uri(FakeEndpoint), new FakeMethod(), new FakeHeader(), null, FakeAction);

            // Verify outcome
            clientMock.VerifyAll();
        }

        [Fact]
        public void CallToPostWithUriEndpointAndSerializer()
        {
            // Setup
            var clientMock = GetClientMock();
            var serializerMock = new Mock<IXElementSerializer>();

            serializerMock
                .Setup(ser => ser.Serialize(It.Is<string>(x => x == "FakeMethod")))
                .Returns(new XElement("FakeMethod"))
                .Verifiable();
            serializerMock
                .Setup(ser => ser.Serialize(It.Is<string>(x => x == "FakeHeader")))
                .Returns(new XElement("FakeHeader"))
                .Verifiable();

            // Exercise
            clientMock.Object.Post(new Uri(FakeEndpoint), "FakeMethod", "FakeHeader", () => serializerMock.Object, FakeAction);

            // Verify outcome
            clientMock.VerifyAll();
            serializerMock.VerifyAll();
        }

        #region Private Methods

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

        private static Mock<ISoapClient> GetClientMockMultiple()
        {
            var clientMock = new Mock<ISoapClient>();

            clientMock
                .Setup(x => x.PostAsync(
                    It.Is<string>(endpoint => endpoint == FakeEndpoint),
                    It.Is<IEnumerable<XElement>>(
                        bodyElements => bodyElements
                            .All(e => e.Name.ToString()
                            .StartsWith("BodyElement"))),
                    It.Is<IEnumerable<XElement>>(
                        headerElements => headerElements
                            .All(e => e.Name.ToString()
                            .StartsWith("HeaderElement"))),
                    It.Is<string>(action => action == FakeAction)))
                .ReturnsAsync(new HttpResponseMessage())
                .Verifiable();

            return clientMock;
        }

        #endregion Private Methods

        #region DTO

        public class FakeMethod
        {
        }

        public class FakeHeader
        {
        }

        #endregion DTO
    }
}