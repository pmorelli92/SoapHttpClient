using SoapHttpClient.Enums;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SoapHttpClient
{
    public interface ISoapClient
    {
        /// <summary>
        /// Posts an asynchronous message.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="soapVersion">The preferred SOAP version.</param>
        /// <param name="bodies">The body of the SOAP message.</param>
        /// <param name="headers">The header of the SOAP message.</param>
        /// <param name="action">The SOAPAction of the SOAP message.</param>
        Task<HttpResponseMessage> PostAsync(Uri endpoint, SoapVersion soapVersion, IEnumerable<XElement> bodies, IEnumerable<XElement> headers = null, string action = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}