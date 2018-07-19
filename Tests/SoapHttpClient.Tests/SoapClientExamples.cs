using System;
using System.Net;
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
    }
}