using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using SoapHttpClient.Interfaces;

namespace SoapHttpClient.Extensions
{
    public static class ISoapClientExtensions
    {
        /// <summary>
        /// Posts a message.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body of the SOAP message.</param>
        /// <param name="header">The header of the SOAP message.</param>
        public static HttpResponseMessage Post(this ISoapClient @this, string endpoint, XElement body, XElement header = null)
        {
            return @this.PostAsync(endpoint, body, header).Result;
        }

        /// <summary>
        /// Posts an asynchronous message.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body of the SOAP message which will be serialized to XElement.</param>
        /// <param name="header">The header of the SOAP message which wil be serialized to XElement.</param>
        /// <param name="xElementSerializerFactory">Allows the user to define a custom IXElementSerializer instance which will be used for serialization</param>
        public static async Task<HttpResponseMessage> PostAsync(
            this ISoapClient @this,
            string endpoint,
            object body,
            object header = null,
            Func<IXElementSerializer> xElementSerializerFactory = null)
        {
            if (body == null)
                throw new ArgumentNullException("body");

            if (xElementSerializerFactory == null)
                xElementSerializerFactory = () => new XElementSerializer();

            var xElementSerializer = xElementSerializerFactory();

            var headerElement = default(XElement);

            if (header != null)
                headerElement = xElementSerializer.Serialize(header);

            return await @this.PostAsync(endpoint, xElementSerializer.Serialize(body), headerElement);
        }

        /// <summary>
        /// Posts a message.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body of the SOAP message which will be serialized to XElement.</param>
        /// <param name="header">The header of the SOAP message which wil be serialized to XElement.</param>
        /// <param name="xElementSerializerFactory">Allows the user to define a custom IXElementSerializer instance which will be used for serialization</param>
        public static HttpResponseMessage PostMessage(
            this ISoapClient @this,
            string endpoint,
            object body,
            object header = null,
            Func<IXElementSerializer> xElementSerializerFactory = null)
        {
            return @this.PostAsync(endpoint, body, header, xElementSerializerFactory).Result;
        }
    }
}