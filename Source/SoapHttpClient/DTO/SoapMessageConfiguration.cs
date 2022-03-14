using SoapHttpClient.Enums;
using System.Xml.Linq;

namespace SoapHttpClient.DTO;

public record SoapMessageConfiguration(SoapVersion SoapVersion)
{
    public string MediaType =>
        SoapVersion == SoapVersion.Soap11
            ? "text/xml"
            : "application/soap+xml";

    public XNamespace Schema =>
        SoapVersion == SoapVersion.Soap11
            ? "http://schemas.xmlsoap.org/soap/envelope/"
            : "http://www.w3.org/2003/05/soap-envelope";
}
