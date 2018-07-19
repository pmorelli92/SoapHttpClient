using System;
using System.Net.Http.Headers;

namespace SoapHttpClient.Tests.Fixtures
{
    public class TestCall
    {
        public Uri Uri { get; }

        public string Body { get; }

        public HttpContentHeaders Headers { get; }

        public TestCall(Uri uri, string body, HttpContentHeaders headers)
        {
            Uri = uri;
            Body = body;
            Headers = headers;
        }
    }
}
