using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SoapHttpClient.Tests.Fixtures
{
    public class TestHttpClientFactory : IHttpClientFactory
    {
        public TestMessageHandler Handler { get; }

        public TestHttpClientFactory()
        {
            Handler = new TestMessageHandler();
        }

        public HttpClient CreateClient(string name)
        {
            return new HttpClient(Handler);
        }
    }

    public class TestMessageHandler : HttpMessageHandler
    {
        public List<TestCall> CallStack { get; set; }

        public TestMessageHandler()
            => CallStack = new List<TestCall>();

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            CallStack.Add(
                new TestCall(
                    request.RequestUri,
                    await request.Content.ReadAsStringAsync(),
                    request.Content.Headers));

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
