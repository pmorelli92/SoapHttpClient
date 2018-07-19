using System.Xml.Linq;
using System.Xml.Serialization;

namespace SoapHttpClient
{
    public class XElementSerializer : IXElementSerializer
    {
        /// <inheritdoc />
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