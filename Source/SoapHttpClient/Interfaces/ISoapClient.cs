using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SoapHttpClient.Interfaces
{
    public interface ISoapClient
    {
        /// <summary>
        ///     Posts an asynchronous message.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body of the SOAP message.</param>
        /// <param name="header">The header of the SOAP message.</param>
        /// <param name="action">The SOAPAction of the SOAP message.</param>
        Task<HttpResponseMessage> PostAsync(string endpoint, XElement body, XElement header = null, string action = null);
    }
}