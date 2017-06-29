using SoapHttpClient.Enums;
using System.Xml.Linq;

namespace SoapHttpClient.DTO
{
    public class SoapMessageConfiguration
    {
        public string MediaType { get; private set; }

        public XNamespace Schema { get; private set; }

        public SoapVersion SoapVersion { get; private set; }

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
