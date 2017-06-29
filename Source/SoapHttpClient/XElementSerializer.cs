using System.Xml.Linq;
using System.Xml.Serialization;

namespace SoapHttpClient
{
    public class XElementSerializer : IXElementSerializer
    {
        /// <summary>
        /// Serializes the specified object to XElement
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        public XElement Serialize(object obj)
        {
            var xs = new XmlSerializer(obj.GetType());

            var xDoc = new XDocument();

            using (var xw = xDoc.CreateWriter())
                xs.Serialize(xw, obj);

            return xDoc.Root;
        }
    }
}