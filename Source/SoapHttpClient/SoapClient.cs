using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
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

        /// <inheritdoc />
        public Task<HttpResponseMessage> PostAsync(
            Uri endpoint, 
            SoapVersion soapVersion,
            IEnumerable<XElement> bodies,
            IEnumerable<XElement> headers = null,
            string action = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _httpClient.PostSoapAsync(endpoint, soapVersion, bodies, headers, action, cancellationToken);
        }

        private static HttpClient DefaultHttpFactory()
            => new HttpClient(new HttpClientHandler {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            }, disposeHandler: false);
    }
}