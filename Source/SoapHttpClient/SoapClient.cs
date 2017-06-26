using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SoapHttpClient.Interfaces;

namespace SoapHttpClient
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class SoapClient : ISoapClient, IDisposable
    {
        private const string Body = "Body";
        private const string Header = "Header";
        private const string Prefix = "soapenv";
        private const string Envelope = "Envelope";
        private const string ActionHeader = "SOAPAction";
        private const string ActionParameter = "action";

        private readonly HttpClient _httpClient;
        private readonly string _mediaType;
        private readonly XNamespace _soapSchema;
        private readonly SoapVersion _version;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SoapClient" /> class.
        /// </summary>
        /// <param name="httpClientFactory">Allows the user to define the construction of the HttpClient</param>
        /// <param name="version">Optionally provide a preferred SOAP version. Defaults to SOAP 1.1.</param>
        public SoapClient(Func<HttpClient> httpClientFactory, SoapVersion version = SoapVersion.Soap11)
        {
            _version = version;
            switch (version)
            {
                case SoapVersion.Soap11:
                    _soapSchema = "http://schemas.xmlsoap.org/soap/envelope/";
                    _mediaType = "text/xml";
                    break;
                case SoapVersion.Soap12:
                    _soapSchema = "http://www.w3.org/2003/05/soap-envelope";
                    _mediaType = "application/soap+xml";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version,
                        $"The {version.GetType().Name} enum contains an unsupported value.");
            }

            _httpClient = httpClientFactory();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SoapClient" /> class.
        ///     The internal HttpClient supports AutomaticDecompression of GZip and Deflate
        /// </summary>
        public SoapClient(SoapVersion version = SoapVersion.Soap11) : this(DefaultHttpFactory, version)
        {
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        /// <summary>
        ///     Posts an asynchronous message.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body of the SOAP message.</param>
        /// <param name="header">The header of the SOAP message.</param>
        /// <param name="action"></param>
        public Task<HttpResponseMessage> PostAsync(string endpoint, XElement body, XElement header = null,
            string action = null)
        {
            if (body == null)
                throw new ArgumentNullException(Body);

            var soapMessage = GetSoapMessage(header, body);
            return PostAsync(endpoint, soapMessage, action);
        }

        public Task<HttpResponseMessage> PostAsync(string endpoint, IEnumerable<XElement> bodyElements,
            IEnumerable<XElement> headerElements, string action = null)
        {
            var soapMessage = GetSoapMessage(headerElements, bodyElements);
            return PostAsync(endpoint, soapMessage, action);
        }

        #region Private Methods

        private static HttpClient DefaultHttpFactory()
        {
            var client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

            return client;
        }

        private Task<HttpResponseMessage> PostAsync(string endpoint, string soapMessage, string action = null)
        {
            var content = new StringContent(soapMessage, Encoding.UTF8, _mediaType);

            if (action == null)
                return _httpClient.PostAsync(endpoint, content);

            if (_version == SoapVersion.Soap11)
                content.Headers.Add(ActionHeader, action);

            if (_version == SoapVersion.Soap12)
                content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue(ActionParameter, $"\"{action}\""));

            return _httpClient.PostAsync(endpoint, content);
        }

        private string GetSoapMessage(XElement header, XElement body)
        {
            var soapMessage = new XElement(
                _soapSchema + Envelope,
                new XAttribute(
                    XNamespace.Xmlns + Prefix,
                    _soapSchema.NamespaceName)
                );

            if (header != null)
                soapMessage.Add(new XElement(_soapSchema + Header, header));

            soapMessage.Add(new XElement(_soapSchema + Body, body));

            return new XElement(soapMessage).ToString();
        }

        private string GetSoapMessage(IEnumerable<XElement> headerElements, IEnumerable<XElement> bodyElements)
        {
            var soapMessage = new XElement(
                _soapSchema + Envelope,
                new XAttribute(
                    XNamespace.Xmlns + Prefix,
                    _soapSchema.NamespaceName)
            );

            if (headerElements.Any())
                soapMessage.Add(new XElement(_soapSchema + Header, headerElements));

            soapMessage.Add(new XElement(_soapSchema + Body, bodyElements));

            return new XElement(soapMessage).ToString();
        }

        #endregion Private Methods
    }
}