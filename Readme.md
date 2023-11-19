# How to using?

## Using 1 - Returned string reponse content

```cs
    
    HttpResponseMessage response;

    using (HttpClientSender client = new HttpClientSender())
    {
        client.Url = new Uri("<UrlAddress>/AddEmployee");
        
        //Adding no-cache headers        
        client.AddDisabledCacheHeaders();

        //Adding custom headers
        client.AddHeader("Authorization", "Bearer <token-here>");
        client.AddHeader("Content-Type", "application/json; charset=utf-8;");
        client.AddHeader("My-Custom-Header", "My-Custom-Value");
        client.AddHeader("My-Custom-Header-2", "My-Custom-Value-2");

        //Adding body content (dynamic object)
        client.AddContent(new
        {
            Name    = "Mevlut",
            Surname = "Can",
            Age     = 21
            Job     = "Developer"
        }, MediaType.Json);

        // OR

        client.AddContent(new Person 
        {
            Name    = "Mevlut",
            Surname = "Can",
            Age     = 21
            Job     = "Developer"
        }, MediaType.Json);

        response = await client.SendAsync(HttpMethod.Post);
    }

    var contentString = response.GetContent();
    Console.WriteLine(contentString);

```


## Using 2 - Returned model reponse content

```cs
    
    MyResponseModel response;

    using (HttpClientSender client = new HttpClientSender())
    {
        client.Url = new Uri("<UrlAddress>/AddEmployee");
        
        //Adding no-cache headers        
        client.AddDisabledCacheHeaders();

        //Adding custom headers
        client.AddHeader("Authorization", "Bearer <token-here>");
        client.AddHeader("Content-Type", "application/json; charset=utf-8;");
        client.AddHeader("My-Custom-Header", "My-Custom-Value");
        client.AddHeader("My-Custom-Header-2", "My-Custom-Value-2");

        //Adding body content (dynamic object)
        client.AddContent(new
        {
            Name    = "Mevlut",
            Surname = "Can",
            Age     = 21
            Job     = "Developer"
        }, MediaType.Json);

        // OR

        client.AddContent(new Person 
        {
            Name    = "Mevlut",
            Surname = "Can",
            Age     = 21
            Job     = "Developer"
        }, MediaType.Json);

        response = await client.SendAsync<MyResponseModel>(HttpMethod.Post);
    }

    Console.WriteLine($"Name:{response.Name}, Surname: {response.Surname}");

```


>### Exception information that can be thrown;

>```RequestFailException``` Exception is thrown if anything other than 200 status code is returned from the relevant endpoint address. For example; 400, 500, 401, 404 etc.

> ```RequestErrorException``` Exception is thrown when the relevant url address cannot be reached. As the simplest example; It is a type of exception that is thrown in errors such as 'The connection could not be established because the target machine actively rejected it'.

>```ArgumentNullException``` Exception is thrown when the Url address or body content fields are null or invalid.

>```JsonException```Exception is thrown when the response json data returned from the API is not converted to the desired C# model.


### Example RequestFailException exception result;

```cs

HttpLibrary.Infrastructer.Exceptions.RequestFailException: <!DOCTYPE html>
<html lang="en">
<head>
<meta charset="utf-8">
<title>Error</title>
</head>
<body>
<pre>Cannot PUT /user/7</pre>
</body>
</html>

   at HttpLibrary.HttpClientSender.SendAsync(HttpMethod method) in C:\HttpLibrary\HttpLibrary\HttpClientSender.cs:line 169
   at Program.<<Main>$>g__Request_PUT|0_2() in C:\HttpLibrary\ExampleApp\Program.cs:line 90

```



