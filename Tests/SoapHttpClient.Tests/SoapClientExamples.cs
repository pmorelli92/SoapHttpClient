using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using FluentAssertions;
using SoapHttpClient.Enums;
using Xunit;

namespace SoapHttpClient.Tests
{
    public class SoapClientExamples
    {
        [Fact(DisplayName = "Get NASA's heliocentric trajectories (Soap 1.1)")]
        public async void Nasa_Trajectory_Soap11()
        {
            var soapClient = new SoapClient();
            var ns = XNamespace.Get("http://helio.spdf.gsfc.nasa.gov/");

            var result =
                await soapClient.PostAsync(
                    new Uri("http://sscweb.gsfc.nasa.gov:80/WS/helio/1/HeliocentricTrajectoriesService"),
                    SoapVersion.Soap11,
                    body: new XElement(ns.GetName("getAllObjects")));

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact(DisplayName = "Cancel NASA's heliocentric trajectories (Soap 1.1) Operation Cancelled")]
        public async void Nasa_Cancel_Trajectory_Soap11_OperationCancelled()
        {
            var soapClient = new SoapClient();
            var ns = XNamespace.Get("http://helio.spdf.gsfc.nasa.gov/");
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var task = soapClient.PostAsync(
                new Uri("http://sscweb.gsfc.nasa.gov:80/WS/helio/1/HeliocentricTrajectoriesService"),
                SoapVersion.Soap11,
                body: new XElement(ns.GetName("getAllObjects")),
                cancellationToken: token);

            tokenSource.Cancel(true);

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
        }

        [Fact(DisplayName = "Cancel NASA's heliocentric trajectories (Soap 1.1) Task Cancelled")]
        public void Nasa_Cancel_Trajectory_Soap11_TaskCancelled()
        {
            var soapClient = new SoapClient();
            var ns = XNamespace.Get("http://helio.spdf.gsfc.nasa.gov/");
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            Assert.ThrowsAsync<TaskCanceledException>(() =>
                soapClient.PostAsync(
                    new Uri("http://sscweb.gsfc.nasa.gov:80/WS/helio/1/HeliocentricTrajectoriesService"),
                    SoapVersion.Soap11,
                    body: new XElement(ns.GetName("getAllObjects")),
                    cancellationToken: token));

            tokenSource.Cancel(true);
        }
    }
}