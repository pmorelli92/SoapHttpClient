using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using SoapHttpClient.DTO;
using SoapHttpClient.Enums;

namespace SoapHttpClient
{
    public class SoapMessageBuilder
    {
        private readonly IList<XElement> _bodies = new List<XElement>();
        private readonly IList<XElement> _headers = new List<XElement>();

        public ICollection<XElement> Bodies => _bodies;
        public ICollection<XElement> Headers => _headers;
        public string SoapAction { get; set; }
        public Uri EndpointUri { get; set; }
        public SoapVersion SoapVersion { get; set; }
        
        public HttpRequestMessage Build()
        {
            if (EndpointUri == null)
                throw new InvalidOperationException($"{nameof(EndpointUri)} property not set");

            if (!Bodies.Any())
                throw new InvalidOperationException("Bodies property cannot be empty");

            // Get configuration based on version
            var messageConfiguration = new SoapMessageConfiguration(SoapVersion);

            // Get the envelope
            var envelope = GetEnvelope(messageConfiguration);

            // Add headers
            if (Headers.Any())
                envelope.Add(new XElement(messageConfiguration.Schema + "Header", Headers));

            // Add bodies
            envelope.Add(new XElement(messageConfiguration.Schema + "Body", Bodies));

            // Get HTTP content
            var content = new StringContent(envelope.ToString(), Encoding.UTF8, messageConfiguration.MediaType);

            // Add SOAP action if any
            if (SoapAction != null)
            {
                content.Headers.Add("SOAPAction", SoapAction);

                if (messageConfiguration.SoapVersion == SoapVersion.Soap12)
                    content.Headers.ContentType.Parameters.Add(
                        new NameValueHeaderValue("ActionParameter", $"\"{SoapAction}\""));
            }
            
            return new HttpRequestMessage(HttpMethod.Post, EndpointUri)
            {
                Content = content
            };
        }

        private static XElement GetEnvelope(SoapMessageConfiguration soapMessageConfiguration)
        {
            return new 
                XElement(
                    soapMessageConfiguration.Schema + "Envelope",
                    new XAttribute(
                        XNamespace.Xmlns + "soapenv",
                        soapMessageConfiguration.Schema.NamespaceName));
        }
    }
}