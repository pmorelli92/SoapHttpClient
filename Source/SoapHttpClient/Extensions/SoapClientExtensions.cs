using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using SoapHttpClient.Interfaces;

namespace SoapHttpClient.Extensions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class SoapClientExtensions
    {
        /// <summary>
        ///     Posts a message.
        /// </summary>
        /// <param name="client">Instance of SoapClient</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body of the SOAP message.</param>
        /// <param name="header">The header of the SOAP message.</param>
        public static async Task<HttpResponseMessage> PostAsync(
            this ISoapClient client,
            Uri endpoint,
            XElement body,
            XElement header = null,
            string action = null) {
            return await client.PostAsync(endpoint.ToString(), body, header, action);
        }

        /// <summary>
        ///     Posts a message.
        /// </summary>
        /// <param name="client">Instance of SoapClient</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body of the SOAP message.</param>
        /// <param name="header">The header of the SOAP message.</param>
        public static HttpResponseMessage Post(
            this ISoapClient client,
            string endpoint,
            XElement body,
            XElement header = null,
            string action = null)
        {
            return client.PostAsync(endpoint, body, header, action).Result;
        }

        /// <summary>
        ///     Posts a message.
        /// </summary>
        /// <param name="client">Instance of SoapClient</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body of the SOAP message.</param>
        /// <param name="header">The header of the SOAP message.</param>
        public static HttpResponseMessage Post(
            this ISoapClient client,
            Uri endpoint,
            XElement body,
            XElement header = null,
            string action = null)
        {
            return client.PostAsync(endpoint.ToString(), body, header, action).Result;
        }

        /// <summary>
        ///     Posts an asynchronous message.
        /// </summary>
        /// <param name="client">Instance of SoapClient</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body of the SOAP message which will be serialized to XElement.</param>
        /// <param name="header">The header of the SOAP message which wil be serialized to XElement.</param>
        /// <param name="xElementSerializerFactory">
        ///     Allows the user to define a custom IXElementSerializer instance which will be
        ///     used for serialization
        /// </param>
        public static async Task<HttpResponseMessage> PostAsync(
            this ISoapClient client,
            string endpoint,
            object body,
            object header = null,
            Func<IXElementSerializer> xElementSerializerFactory = null,
            string action = null)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            if (xElementSerializerFactory == null)
                xElementSerializerFactory = () => new XElementSerializer();

            var xElementSerializer = xElementSerializerFactory();

            var headerElement = default(XElement);

            if (header != null)
                headerElement = xElementSerializer.Serialize(header);

            return await client.PostAsync(endpoint, xElementSerializer.Serialize(body), headerElement, action);
        }

        /// <summary>
        ///     Posts an asynchronous message.
        /// </summary>
        /// <param name="client">Instance of SoapClient</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body of the SOAP message which will be serialized to XElement.</param>
        /// <param name="header">The header of the SOAP message which wil be serialized to XElement.</param>
        /// <param name="xElementSerializerFactory">
        ///     Allows the user to define a custom IXElementSerializer instance which will be
        ///     used for serialization
        /// </param>
        public static async Task<HttpResponseMessage> PostAsync(
            this ISoapClient client,
            Uri endpoint,
            object body,
            object header = null,
            Func<IXElementSerializer> xElementSerializerFactory = null,
            string action = null)
        {
            return await client.PostAsync(endpoint.ToString(), body, header, xElementSerializerFactory, action);
        }

        /// <summary>
        ///     Posts a message.
        /// </summary>
        /// <param name="client">Instance of SoapClient</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body of the SOAP message which will be serialized to XElement.</param>
        /// <param name="header">The header of the SOAP message which wil be serialized to XElement.</param>
        /// <param name="xElementSerializerFactory">
        ///     Allows the user to define a custom IXElementSerializer instance which will be
        ///     used for serialization
        /// </param>
        public static HttpResponseMessage Post(
            this ISoapClient client,
            string endpoint,
            object body,
            object header = null,
            Func<IXElementSerializer> xElementSerializerFactory = null,
            string action = null)
        {
            return client.PostAsync(endpoint, body, header, xElementSerializerFactory, action).Result;
        }

        /// <summary>
        ///     Posts a message.
        /// </summary>
        /// <param name="client">Instance of SoapClient</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body of the SOAP message which will be serialized to XElement.</param>
        /// <param name="header">The header of the SOAP message which wil be serialized to XElement.</param>
        /// <param name="xElementSerializerFactory">
        ///     Allows the user to define a custom IXElementSerializer instance which will be
        ///     used for serialization
        /// </param>
        public static HttpResponseMessage Post(
            this ISoapClient client,
            Uri endpoint,
            object body,
            object header = null,
            Func<IXElementSerializer> xElementSerializerFactory = null,
            string action = null)
        {
            return client.Post(endpoint.ToString(), body, header, xElementSerializerFactory, action);
        }

        [Obsolete("PostMessage has been depricated and will be removed in a future version.  Please replace all calls to PostMessage with calls to the overload of Post.")]
        public static HttpResponseMessage PostMessage(
            this ISoapClient @this,
            string endpoint,
            object body,
            object header = null,
            Func<IXElementSerializer> xElementSerializerFactory = null) {
            return @this.Post(endpoint, body, header, xElementSerializerFactory);
        }
    }
}