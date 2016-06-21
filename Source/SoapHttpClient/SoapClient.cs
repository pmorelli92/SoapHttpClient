using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SoapHttpClient.Interfaces;

namespace SoapHttpClient
{
    public class SoapClient : ISoapClient, IDisposable
    {
        private readonly string Body = "Body";
        private readonly string Header = "Header";
        private readonly string Prefix = "soapenv";
        private readonly string Envelope = "Envelope";
        private readonly XNamespace SoapSchema = "http://schemas.xmlsoap.org/soap/envelope/";
        private readonly string ApplicationXml = "application/xml";

        private HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoapClient"/> class.
        /// </summary>
        /// <param name="httpClientFactory">Allows the user to define the construction of the HttpClient</param>
        public SoapClient(Func<HttpClient> httpClientFactory)
        {
            _httpClient = httpClientFactory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoapClient"/> class.
        /// The internal HttpClient supports AutomaticDecompression of GZip and Deflate
        /// </summary>
        public SoapClient()
            : this(() =>
            {
                var client = new HttpClient(
                                new HttpClientHandler()
                                {
                                    AutomaticDecompression =
                                        DecompressionMethods.GZip |
                                        DecompressionMethods.Deflate
                                });

                return client;
            })
        { }

        /// <summary>
        /// Posts an asynchronous message.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body of the SOAP message.</param>
        /// <param name="header">The header of the SOAP message.</param>
        public async Task<HttpResponseMessage> PostAsync(string endpoint, XElement body, XElement header = null)
        {
            if (body == null)
                throw new ArgumentNullException(Body);

            var soapMessage = GetSoapMessage(header, body);
            var content = new StringContent(soapMessage, Encoding.UTF8, ApplicationXml);

            return await _httpClient.PostAsync(endpoint, content);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        #region Private Methods

        private string GetSoapMessage(XElement header, XElement body)
        {
            return new XElement(
                        new XElement(
                            SoapSchema + Envelope,
                            new XAttribute(
                                XNamespace.Xmlns + Prefix,
                                SoapSchema.NamespaceName),
                            new XElement(
                                SoapSchema + Header,
                                header),
                            new XElement(
                                SoapSchema + Body,
                                body))).ToString();
        }

        #endregion Private Methods
    }
}