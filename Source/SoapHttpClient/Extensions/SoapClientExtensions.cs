using SoapHttpClient.Enums;
using System.Xml.Linq;

namespace SoapHttpClient
{
    public static class SoapClientExtensions
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
        public static Task<HttpResponseMessage> PostAsync(
            this ISoapClient client,
            Uri endpoint,
            SoapVersion soapVersion,
            XElement body,
            XElement? header = null,
            string? action = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return client.PostAsync(
                endpoint,
                soapVersion,
                new[] { body },
                header != null ? new[] { header } : default(IEnumerable<XElement>),
                action,
                cancellationToken);
        }

        /// <summary>
        /// Posts an asynchronous message.
        /// </summary>
        /// <param name="client">Instance of SoapClient.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="soapVersion">The version of SOAP</param>
        /// <param name="bodies">The bodies of the SOAP message.</param>
        /// <param name="header">The header of the SOAP message.</param>
        /// <param name="action"></param>
        public static Task<HttpResponseMessage> PostAsync(
            this ISoapClient client,
            Uri endpoint,
            SoapVersion soapVersion,
            IEnumerable<XElement> bodies,
            XElement header,
            string? action = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return client.PostAsync(
                endpoint,
                soapVersion,
                bodies,
                new[] { header },
                action,
                cancellationToken);
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
        public static Task<HttpResponseMessage> PostAsync(
            this ISoapClient client,
            Uri endpoint,
            SoapVersion soapVersion,
            XElement body,
            IEnumerable<XElement> headers,
            string? action = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return client.PostAsync(
                endpoint,
                soapVersion,
                new[] { body },
                headers,
                action,
                cancellationToken);
        }
    }
}
