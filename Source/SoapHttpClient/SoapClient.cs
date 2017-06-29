using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SoapHttpClient.Enums;
using SoapHttpClient.DTO;

namespace SoapHttpClient
{
    public class SoapClient : ISoapClient, IDisposable
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoapClient" /> class.
        /// </summary>
        /// <param name="httpClientFactory">Allows the user to define the construction of the HttpClient</param>
        public SoapClient(Func<HttpClient> httpClientFactory)
            => _httpClient = httpClientFactory();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SoapClient" /> class.
        /// The internal HttpClient supports AutomaticDecompression of GZip and Deflate
        /// </summary>
        public SoapClient()
            : this(DefaultHttpFactory)
        {
        }

        public void Dispose()
            => _httpClient.Dispose();

        /// <summary>
        /// Posts an asynchronous message.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="version">The preferred SOAP version.</param>
        /// <param name="bodies">The body of the SOAP message.</param>
        /// <param name="headers">The header of the SOAP message.</param>
        /// <param name="action">The SOAPAction of the SOAP message.</param>
        public Task<HttpResponseMessage> PostAsync(
            Uri endpoint, 
            SoapVersion soapVersion,
            IEnumerable<XElement> bodies,
            IEnumerable<XElement> headers = null,
            string action = null)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            if (bodies == null)
                throw new ArgumentNullException(nameof(bodies));

            if (!bodies.Any())
                throw new ArgumentException("Bodies element cannot be empty", nameof(bodies));

            // Get configuration based on version
            var messageConfiguration = new SoapMessageConfiguration(soapVersion);

            // Get the envelope
            var envelope = GetEnvelope(messageConfiguration);

            // Add headers
            if (headers != null && headers.Any())
                envelope.Add(new XElement(messageConfiguration.Schema + "Header", headers));

            // Add bodies
            envelope.Add(new XElement(messageConfiguration.Schema + "Body", bodies));

            // Get HTTP content
            var content = new StringContent(envelope.ToString(), Encoding.UTF8, messageConfiguration.MediaType);

            // Add SOAP action if any
            if (action != null && messageConfiguration.SoapVersion == SoapVersion.Soap11)
                content.Headers.Add("ActionHeader", action);
            else if (action != null && messageConfiguration.SoapVersion == SoapVersion.Soap12)
                content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("ActionParameter", $"\"{action}\""));

            // Execute call
            return _httpClient.PostAsync(endpoint, content);
        }

        #region Private Methods

        private XElement GetEnvelope(SoapMessageConfiguration soapMessageConfiguration)
        {
            return new 
                XElement(
                    soapMessageConfiguration.Schema + "Envelope",
                    new XAttribute(
                        XNamespace.Xmlns + "soapenv",
                        soapMessageConfiguration.Schema.NamespaceName));
        }

        private static HttpClient DefaultHttpFactory()
            => new HttpClient(new HttpClientHandler {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            }, disposeHandler: false);

        #endregion Private Methods
    }
}