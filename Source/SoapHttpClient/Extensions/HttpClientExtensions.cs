using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using SoapHttpClient.Enums;

namespace SoapHttpClient
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostSoapAsync(this HttpClient client,
            Uri endpoint,
            SoapVersion soapVersion,
            IEnumerable<XElement> bodies,
            IEnumerable<XElement> headers = null,
            string action = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            if (bodies == null)
                throw new ArgumentNullException(nameof(bodies));

            var builder = new SoapMessageBuilder
            {
                EndpointUri = endpoint,
                SoapAction = action,
                SoapVersion = soapVersion
            };

            foreach (var element in bodies)
                builder.Bodies.Add(element);

            foreach (var element in headers ?? new XElement[0])
                builder.Headers.Add(element);

            var message = builder.Build();

            return client.SendAsync(message, cancellationToken);
        }
    }
}