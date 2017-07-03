using System;
using System.Net.Http.Headers;

namespace SoapHttpClient.Fixtures
{
    public class TestCall
    {
        public Uri Uri { get; private set; }

        public string Body { get; private set; }

        public HttpContentHeaders Headers { get; private set; }

        public TestCall(Uri uri, string body, HttpContentHeaders headers)
        {
            Uri = uri;
            Body = body;
            Headers = headers;
        }
    }
}
