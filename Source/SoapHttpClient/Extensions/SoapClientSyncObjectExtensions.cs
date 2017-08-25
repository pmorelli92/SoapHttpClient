using SoapHttpClient.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;

namespace SoapHttpClient.Extensions
{
    public static class SoapClientSyncObjectExtensions
    {
        /// <summary>
        /// Posts an asynchronous message.
        /// </summary>
        /// <param name="client">Instance of SoapClient.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body of the SOAP message.</param>
        /// <param name="header">The header of the SOAP message.</param>
        /// <param name="action"></param>
        public static HttpResponseMessage Post(
            this ISoapClient client,
            Uri endpoint,
            SoapVersion soapVersion,
            object body,
            object header = null,
            IXElementSerializer xElementSerializer = null,
            string action = null)
        {
            if (xElementSerializer == null)
                xElementSerializer = new XElementSerializer();

            return SoapClientSyncExtensions.Post(
                client,
                endpoint,
                soapVersion,
                xElementSerializer.Serialize(body),
                header != null ? new[] { xElementSerializer.Serialize(header) } : default(IEnumerable<XElement>));
        }

        /// <summary>
        /// Posts an asynchronous message.
        /// </summary>
        /// <param name="client">Instance of SoapClient.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body of the SOAP message.</param>
        /// <param name="header">The header of the SOAP message.</param>
        /// <param name="action"></param>
        public static HttpResponseMessage Post(
            this ISoapClient client,
            Uri endpoint,
            SoapVersion soapVersion,
            IEnumerable<object> bodies,
            object header,
            IXElementSerializer xElementSerializer = null,
            string action = null)
        {
            if (xElementSerializer == null)
                xElementSerializer = new XElementSerializer();

            return SoapClientSyncExtensions.Post(
                client,
                endpoint,
                soapVersion,
                bodies.Select(e => xElementSerializer.Serialize(e)),
                header != null ? xElementSerializer.Serialize(header) : default(XElement),
                action);
        }

        /// <summary>
        /// Posts an asynchronous message.
        /// </summary>
        /// <param name="client">Instance of SoapClient.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body of the SOAP message.</param>
        /// <param name="header">The header of the SOAP message.</param>
        /// <param name="action"></param>
        public static HttpResponseMessage Post(
            this ISoapClient client,
            Uri endpoint,
            SoapVersion soapVersion,
            object body,
            IEnumerable<object> headers,
            IXElementSerializer xElementSerializer = null,
            string action = null)
        {
            if (xElementSerializer == null)
                xElementSerializer = new XElementSerializer();

            return SoapClientSyncExtensions.Post(
                client,
                endpoint,
                soapVersion,
                xElementSerializer.Serialize(body),
                headers.Select(e => xElementSerializer.Serialize(e)),
                action);
        }
    }
}