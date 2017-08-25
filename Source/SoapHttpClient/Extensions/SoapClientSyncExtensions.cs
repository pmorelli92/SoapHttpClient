using SoapHttpClient.Enums;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SoapHttpClient.Extensions
{
    public static class SoapClientSyncExtensions
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
            XElement body,
            XElement header = null,
            string action = null)
                => ResolveTask(() =>
                    SoapClientExtensions.PostAsync(
                        client, endpoint, soapVersion, body, header, action));

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
            IEnumerable<XElement> bodies,
            XElement header,
            string action = null)
                => ResolveTask(() =>
                    SoapClientExtensions.PostAsync(
                        client, endpoint, soapVersion, bodies, header, action));

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
            XElement body,
            IEnumerable<XElement> headers,
            string action = null)
                => ResolveTask(() =>
                    SoapClientExtensions.PostAsync(
                        client, endpoint, soapVersion, body, headers, action));

        #region Private Methods

        private static HttpResponseMessage ResolveTask(Func<Task<HttpResponseMessage>> fn)
        {
            return Task.Run(() => fn()).Result;
        }

        #endregion Private Methods
    }
}