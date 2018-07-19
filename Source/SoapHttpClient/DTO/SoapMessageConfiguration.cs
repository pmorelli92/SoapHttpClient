using SoapHttpClient.Enums;
using System.Xml.Linq;

namespace SoapHttpClient.DTO
{
    public class SoapMessageConfiguration
    {
        public string MediaType { get; }

        public XNamespace Schema { get; }

        public SoapVersion SoapVersion { get; }

        public SoapMessageConfiguration(SoapVersion soapVersion)
        {
            if (soapVersion == SoapVersion.Soap11)
            {
                MediaType = "text/xml";
                Schema = "http://schemas.xmlsoap.org/soap/envelope/";
            }
            else
            {
                MediaType = "application/soap+xml";
                Schema = "http://www.w3.org/2003/05/soap-envelope";
            }

            SoapVersion = soapVersion;
        }
    }
}
