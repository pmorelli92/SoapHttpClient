#SoapClient
----------

####A lightweight wrapper of an HttpClient for POSTing messages that allows the user to send the SOAP Body and Header (if needed) without caring about the envelope.

----------

###Methods

	Ext.Post(string endpoint, XElement body, XElement header = null)
    PostAsync(string endpoint, XElement body, XElement header = null)
	Ext.Post(string endpoint, object body, object header = null, Func<IXElementSerializer> xElementSerializerFactory = null)
	Ext.PostAsync(string endpoint, object body, object header = null, Func<IXElementSerializer> xElementSerializerFactory = null)

**Extended methods are to be found in the SoapClient.Extensions namespace.**

----------

###Usage Example

    using (var soapClient = new SoapClient(() => new HttpClient()))
    {
        var result = soapClient.PostMessage(
           					endpoint: TestTokenManagerEndpoint,
            				body: myBody);
    }