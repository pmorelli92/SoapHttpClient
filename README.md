# SoapClient

> A lightweight wrapper of an `HttpClient` for POSTing messages that allows the
> user to send the SOAP Body and Header (if needed) without caring about the
> envelope.

## API

### Constructors

```csharp
SoapClient()
```

Initializes `SoapClient` with a default `HttpClientFactory` that implements
automatic decompression for Soap 1.1 version.

```csharp
SoapClient(SoapVersion version)
```

Initializes `SoapClient` with a default `HttpClientFactory` that implements
automatic decompression for Soap 1.2 version.

```csharp
SoapClient(Func<HttpClient> httpClientFactory)
```

Initializes `SoapClient` with a `HttpClientFactory` provided by the caller, a
`HttpClientFactory` is simply a `Func<HttpClient>`, i.e. a function that
returns a `HttpClient`. You can specify if you want to use the Soap 1.2 version, the default is 1.1

### Methods

> All **Methods** and **Extension Methods** return a
> [`HttpResponseMessage`][msdn-httpresponsemessage].

```csharp
HttpResponseMessage PostAsync(
  string endpoint,
  XElement body,
  XElement header = null,
  action = null)
```

Issues the SOAP request asynchronously to the `endpoint`, with the specified
`body` and optional `header` and `action`.

### Extension Methods

> *Extension methods are to be found in the `SoapHttpClient.Extensions`*
> *namespace.*

```csharp
HttpResponseMessage SoapClient.PostAsync(
  Uri endpoint,
  XElement body,
  XElement header = null,
  string action = null);
```

Issues the SOAP request asynchronously to the `endpoint`, with the specified
`body` and optional `header` already serialized, additionally providing an action.

```csharp
HttpResponseMessage SoapClient.Post(
  Uri/string endpoint,
  XElement body,
  XElement header = null,
  string action = null);
```

Issues the SOAP request synchronously to the `endpoint`, with the specified
`body` and optional `header` already serialized, additionally providing an action.

```csharp
HttpResponseMessage SoapClient.PostAsync(
  Uri/string endpoint,
  object body,
  object header = null,
  string action = null);
  Func<IXElementSerializer> xElementSerializerFactory = null)
```

Issues the SOAP request asynchronously to the `endpoint`, with the specified
`body` and optional `header` additionally using a `XElementSerializer` to
control the serialization of the `body` and `header`, additionally providing an action.

```csharp
HttpResponseMessage SoapClient.Post(
  Uri/string endpoint,
  object body,
  object header = null,
  string action = null);
  Func<IXElementSerializer> xElementSerializerFactory = null)
```

Issues the SOAP request synchronously to the `endpoint`, with the specified
`body` and optional `header` additionally using a `XElementSerializer` to
control the serialization of the `body` and `header`, additionally providing an action.

## Usage Examples

### Controlling the Media Type

As the `SoapHttpClient` wraps a `HttpClient` you can control all aspects of
the HTTP Request using a `HttpMessageHandler`:

```csharp
public class ContentTypeChangingHandler : DelegatingHandler
{
  public ContentTypeChangingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

  protected async override Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
  {

    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/xml; charset=utf-8");

    return await base.SendAsync(request, cancellationToken);
  }
}
```

### Echo Service

```csharp
var ns = XNamespace.Get("http://www.bccs.uib.no/EchoService.wsdl");
var endpoint = "http://api.bioinfo.no/services/EchoService";
var body = new XElement(ns.GetName("SayHi"));

var messageHandler = new ContentTypeChangingHandler(new HttpClientHandler());
Func<HttpClient> httpClientFactory = () => new HttpClient(messageHandler);

using (var soapClient = new SoapClient(httpClientFactory))
{
  var result = soapClient.Post(
            endpoint: endpoint,
            body: body);
  Console.WriteLine(result.Content.ReadAsStringAsync().Result);
}
```

#### Result of Calling Echo Service

```xml
<?xml version="1.0" encoding="UTF-8"?>
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/">
   <soapenv:Header />
   <soapenv:Body>
      <SayHiResponse xmlns="http://www.bccs.uib.no/EchoService.wsdl">
         <HiResponse xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:nil="true" />
      </SayHiResponse>
   </soapenv:Body>
</soapenv:Envelope>
```

### Parameters

```csharp
void Main()
{
  var ns = XNamespace.Get("http://helio.spdf.gsfc.nasa.gov/");
  var endpoint = "http://sscweb.gsfc.nasa.gov:80/WS/helio/1/HeliocentricTrajectoriesService";
  var body = new XElement(ns.GetName("getAllObjects"));

  using (var soapClient = new SoapClient()) {
    var result = soapClient.Post(
          endpoint: endpoint,
          body: body,
          mediaType: "text/xml");
    Console.WriteLine(result.Content.ReadAsStringAsync().Result);
  }
}
```

#### Result of Calling NASA Heliocentric Trajectories Service

```xml
<?xml version="1.0" encoding="UTF-8"?>
<S:Envelope xmlns:S="http://schemas.xmlsoap.org/soap/envelope/">
   <S:Body>
      <ns2:getAllObjectsResponse xmlns:ns2="http://helio.spdf.gsfc.nasa.gov/">
         <return>
            <endDate>1993-01-01T00:00:00Z</endDate>
            <id>0001</id>
            <name>COMET GRIGG-SKJLP</name>
            <startDate>1992-01-01T00:00:00Z</startDate>
         </return>
         <return>
            <endDate>1996-03-02T00:00:00Z</endDate>
            <id>0002</id>
            <name>COMET H-M-P</name>
            <startDate>1996-01-01T00:00:00Z</startDate>
         </return>
         <return>
            <endDate>1997-12-31T00:00:00Z</endDate>
            <id>0003</id>
            <name>COMET HALE-BOPP</name>
            <startDate>1997-01-01T00:00:00Z</startDate>
         </return>
         .
         .
         .
      </ns2:getAllObjectsResponse>
   </S:Body>
</S:Envelope>
```

  [msdn-httpresponsemessage]: https://msdn.microsoft.com/en-us/library/system.net.http.httpresponsemessage(v=vs.118).aspx
