using System.Net;
using System.Xml.Linq;
using SoapHttpClient.Enums;
using Xunit;

namespace SoapHttpClient.Tests;

public class SoapClientExamples
{
    [Fact(DisplayName = "Get NASA's heliocentric trajectories (Soap 1.1)")]
    public async void Nasa_Trajectory_Soap11()
    {
        var soapClient = new SoapClient();
        var ns = XNamespace.Get("http://helio.spdf.gsfc.nasa.gov/");

        var actual =
            await soapClient.PostAsync(
                new Uri("https://sscweb.gsfc.nasa.gov:443/WS/helio/1/HeliocentricTrajectoriesService"),
                SoapVersion.Soap11,
                body: new XElement(ns.GetName("getAllObjects")));

        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
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
                new Uri("https://sscweb.gsfc.nasa.gov:443/WS/helio/1/HeliocentricTrajectoriesService"),
                SoapVersion.Soap11,
                body: new XElement(ns.GetName("getAllObjects")),
                cancellationToken: token));

        tokenSource.Cancel(true);
    }
}
