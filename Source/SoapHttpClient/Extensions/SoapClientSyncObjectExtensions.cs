using SoapHttpClient.Enums;
using System.Xml.Linq;

namespace SoapHttpClient
{
    public static class SoapClientSyncObjectExtensions
    {
        /// <summary>
        /// Posts an asynchronous message.
        /// </summary>
        /// <param name="client">Instance of SoapClient.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="soapVersion">The version of SOAP</param>
        /// <param name="body">The body of the SOAP message.</param>
        /// <param name="header">The header of the SOAP message.</param>
        /// <param name="action"></param>
        public static HttpResponseMessage Post(
            this ISoapClient client,
            Uri endpoint,
            SoapVersion soapVersion,
            object body,
            object? header = null,
            IXElementSerializer? xElementSerializer = null,
            string? action = null)
        {
            if (xElementSerializer == null)
                xElementSerializer = new XElementSerializer();

            return client.Post(endpoint,
                soapVersion,
                xElementSerializer.Serialize(body),
                header != null ? new[] { xElementSerializer.Serialize(header) } : Enumerable.Empty<XElement>());
        }

        /// <summary>
        /// Posts an asynchronous message.
        /// </summary>
        /// <param name="client">Instance of SoapClient.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="soapVersion">The version of SOAP</param>
        /// <param name="bodies">The body of the SOAP message.</param>
        /// <param name="header">The header of the SOAP message.</param>
        /// <param name="action"></param>
        public static HttpResponseMessage Post(
            this ISoapClient client,
            Uri endpoint,
            SoapVersion soapVersion,
            IEnumerable<object> bodies,
            object header,
            IXElementSerializer? xElementSerializer = null,
            string? action = null)
        {
            if (xElementSerializer == null)
                xElementSerializer = new XElementSerializer();

            return client.Post(endpoint,
                soapVersion,
                bodies.Select(e => xElementSerializer.Serialize(e)),
                xElementSerializer.Serialize(header),
                action);
        }

        /// <summary>
        /// Posts an asynchronous message.
        /// </summary>
        /// <param name="client">Instance of SoapClient.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="soapVersion">The version of SOAP</param>
        /// <param name="body">The body of the SOAP message.</param>
        /// <param name="headers">The headers of the SOAP message.</param>
        /// <param name="action"></param>
        public static HttpResponseMessage Post(
            this ISoapClient client,
            Uri endpoint,
            SoapVersion soapVersion,
            object body,
            IEnumerable<object> headers,
            IXElementSerializer? xElementSerializer = null,
            string? action = null)
        {
            if (xElementSerializer == null)
                xElementSerializer = new XElementSerializer();

            return client.Post(endpoint,
                soapVersion,
                xElementSerializer.Serialize(body),
                headers.Select(e => xElementSerializer.Serialize(e)),
                action);
        }
    }
}
