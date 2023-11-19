#region Imports
using HttpLibrary;
using HttpLibrary.Infrastructer.Exceptions;
using HttpLibrary.Infrastructer.Extension;
using HttpLibrary.Infrastructer.Structs;
#endregion


async Task Request_POST()
{
    try
    {
        HttpResponseMessage response;

        using (HttpClientSender client = new HttpClientSender())
        {
            client.Url = new Uri("https://fakestoreapi.com/carts");

            client.AddDisabledCacheHeaders();
            client.AddHeader("My-Custom-Header-1", "My-Custom-Value-1");

            client.AddContent(new
            {
                userId = 5,
                date = DateTime.Parse("2023-11-19"),
                products = "[{productId:5,quantity:1},{productId:1,quantity:5}]"

            }, MediaType.Json);

            response = await client.SendAsync(HttpMethod.Post);
        }

        var content = await response.GetContent();
        Console.WriteLine(content);

        //Result: {"id":11,"userId":5,"date":"2023-11-19T00:00:00","products":"[{productId:5,quantity:1},{productId:1,quantity:5}]"}
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}


async Task Request_GET()
{
    try
    {
        HttpResponseMessage response;

        using (HttpClientSender client = new HttpClientSender())
        {
            client.Url = new Uri("https://fakestoreapi.com/carts/user/2");

            client.AddDisabledCacheHeaders();
            client.AddHeader("My-Custom-Header-1", "My-Custom-Value-1");

            response = await client.SendAsync(HttpMethod.Get);
        }

        var content = await response.GetContent();
        Console.WriteLine(content);

        //Result: [{"id":3,"userId":2,"date":"2020-03-01T00:00:00.000Z","products":[{"productId":1,"quantity":2},{"productId":9,"quantity":1}],"__v":0}]
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}


async Task Request_PUT()
{
    try
    {
        HttpResponseMessage response;

        using (HttpClientSender client = new HttpClientSender())
        {
            client.Url = new Uri("https://fakestoreapi.com/carts/7");

            client.AddDisabledCacheHeaders();
            client.AddHeader("My-Custom-Header-1", "My-Custom-Value-1");

            client.AddContent(new
            {
                userId      = 3,
                date        = DateTime.Parse("2019-12-10"),
                products    = "[{ productId: 1,quantity: 3}]"
            }, MediaType.Json);

            response = await client.SendAsync(HttpMethod.Put);
        }

        var content = await response.GetContent();
        Console.WriteLine(content);

        //Result: {"id":7,"userId":3,"date":"2019-12-10T00:00:00","products":"[{ productId: 1,quantity: 3}]"}
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}


async Task Request_Failed()
{
    try
    {
        HttpResponseMessage response;

        using (HttpClientSender client = new HttpClientSender())
        {
            client.Url = new Uri("https://fakestoreapi.com/user/7");

            client.AddDisabledCacheHeaders();
            client.AddHeader("My-Custom-Header-1", "My-Custom-Value-1");

            client.AddContent(new
            {
                userId = 3,
                date = DateTime.Parse("2019-12-10"),
                products = "[{ productId: 1,quantity: 3}]"
            }, MediaType.Json);

            response = await client.SendAsync(HttpMethod.Put);
        }

        var content = await response.GetContent();
        Console.WriteLine(content);

        //Result: thrown exception RequestFailException
    }
    catch (RequestFailException e)
    {
        Console.WriteLine($"Request failed!!\r\n\nStatus code: {e.StatusCode} ({e.StatusCodeInt})\r\n\nMessage: {e.Message}");
        Console.WriteLine($"\r\n\n\n");

        Console.WriteLine(e);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}


async Task Request_Failed_2()
{
    try
    {
        HttpResponseMessage response;

        using (HttpClientSender client = new HttpClientSender())
        {
            client.Url = new Uri("https://localhost:1234/user/7");

            client.AddDisabledCacheHeaders();
            client.AddHeader("My-Custom-Header-1", "My-Custom-Value-1");

            client.AddContent(new
            {
                userId = 3,
                date = DateTime.Parse("2019-12-10"),
                products = "[{ productId: 1,quantity: 3}]"
            }, MediaType.Json);

            response = await client.SendAsync(HttpMethod.Put);
        }

        var content = await response.GetContent();
        Console.WriteLine(content);

        //Result: thrown exception RequestFailException
    }
    catch (RequestErrorException e)
    {
        Console.WriteLine($"Request ERROR!!\r\n\nMessage: {e.Message}");
        Console.WriteLine("\r\n\n");
        Console.WriteLine(e);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}


void Main()
{
    //Request_POST().Wait();

    //Request_GET().Wait();

    //Request_PUT().Wait();

    //Request_Failed().Wait();

    Request_Failed_2().Wait();

    Console.Read();
}



Main();