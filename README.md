# SoapHttpClient  [![NuGet](https://img.shields.io/nuget/v/SoapHttpClient.svg)](https://www.nuget.org/packages/SoapHttpClient)

> A lightweight wrapper of an `HttpClient` for POSTing messages that allows the
> user to send the SOAP Body and Header (if needed) without caring about the
> envelope.

## Changelog

### 2.0.0

- Major refactor to the codebase.
- Added the functionality of adding more than one header and/or body in the envelope.
- The ctor will no longer determine the SoapVersion, since it is a message property and the API should be ignorant about this.
- **[BREAKING CHANGE]: SoapVersion is now required for every message.**
- **[BREAKING CHANGE]: Removed methods where the endpoint was a string instead of an uri.**

## API

### Constructors

```csharp
SoapClient()
```

Initializes `SoapClient` with a default `HttpClientFactory` that implements automatic decompression.

```csharp
SoapClient(Func<HttpClient> httpClientFactory)
```

Initializes `SoapClient` with a `HttpClientFactory` provided by the caller.
The `HttpClientFactory` is simply a `Func<HttpClient>` that returns the HttpClient.

This is in order for the consumer to manage all aspects of the HTTP Request using a `HttpMessageHandler` 

------------------

### Methods

> All **Methods** and **Extension Methods** returns a Task of [`HttpResponseMessage`][msdn-httpresponsemessage]

The interface makes the client implement the following method:

```csharp
Task<HttpResponseMessage> PostAsync(
	Uri endpoint, 
	SoapVersion soapVersion, 
	IEnumerable<XElement> bodies, 
	IEnumerable<XElement> headers = null, 
	string action = null);
```

Allowing us to send the following calls:

- Uri / Version / Bodies
- Uri / Version / Bodies / Headers
- Uri / Version / Bodies / Headers / Action

------------------

Then there are sugar sintax extension methods:

```csharp
Task<HttpResponseMessage> PostAsync(
	this ISoapClient client,
	Uri endpoint,
	SoapVersion soapVersion,
	XElement body,
	XElement header = null,
	string action = null);
			
Task<HttpResponseMessage> PostAsync(
	this ISoapClient client,
	Uri endpoint,
	SoapVersion soapVersion,
	IEnumerable<XElement> bodies,
	XElement header,
	string action = null);
			
Task<HttpResponseMessage> PostAsync(
	this ISoapClient client,
	Uri endpoint,
	SoapVersion soapVersion,
	XElement body,
	IEnumerable<XElement> headers,
	string action = null);
```

Allowing us to send the following calls:

- Uri / Version / Body
- Uri / Version / Body / Header
- Uri / Version / Body / Header / Action
- Uri / Version / Bodies / Header
- Uri / Version / Bodies / Header / Action
- Uri / Version / Body / Headers
- Uri / Version / Body / Headers / Action

------------------

With all of these variants we can send a message with:

- 1 Body - 1 Header
- 1 Body - N Headers
- N Bodies - 1 Header
- N Bodies - N Headers

------------------

There are also extension methods for sync calls:
**However we always recommend using async programming when you are able**.

> Their method name is **Post** and their return type is [`HttpResponseMessage`][msdn-httpresponsemessage]

Finally, we have extensions methods for using bodies and headers as objects and using serialization the default or a custom `IXElementSerializer` to serialize those objects to XElement.

## Usage Examples

### Controlling the Media Type

As the `SoapHttpClient` wraps a `HttpClient` you can control all aspects of the HTTP Request using a `HttpMessageHandler`:

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
async Task CallEchoServiceAsync()
{
	var ns = XNamespace.Get("http://www.bccs.uib.no/EchoService.wsdl");
	var endpoint = new Uri("http://api.bioinfo.no/services/EchoService");
	var body = new XElement(ns.GetName("SayHi"));

	var messageHandler = new ContentTypeChangingHandler(new HttpClientHandler());
	Func<HttpClient> httpClientFactory = () => new HttpClient(messageHandler);

	using (var soapClient = new SoapClient(httpClientFactory))
	{
		var result = 
          await soapClient.PostAsync(
          		endpoint: endpoint,
          		soapVersion: SoapVersion.Soap11,
          		body: body);
					
		Console.WriteLine(await result.Content.ReadAsStringAsync());
	}
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

### Call NASA

```csharp
async Task CallNasaAsync()
{
	var ns = XNamespace.Get("http://helio.spdf.gsfc.nasa.gov/");
	var endpoint = new Uri("http://sscweb.gsfc.nasa.gov:80/WS/helio/1/HeliocentricTrajectoriesService");
	var body = new XElement(ns.GetName("getAllObjects"));

	using (var soapClient = new SoapClient())
	{
		var result = 
          await soapClient.PostAsync(
          		endpoint: endpoint,
          		soapVersion: SoapVersion.Soap11,
          		body: body);

		Console.WriteLine(await result.Content.ReadAsStringAsync());
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

[Soap Icon][nounproj-soap] Created by Jakob Vogel from the Noun Project

[msdn-httpresponsemessage]: https://msdn.microsoft.com/en-us/library/system.net.http.httpresponsemessage(v=vs.118).aspx
[nounproj-soap]: https://thenounproject.com/icon/44504/