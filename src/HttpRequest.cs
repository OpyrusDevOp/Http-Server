public class HttpRequest
{
    public HttpMethod Method { get; set; }
    public string Path { get; set; } = null!;
    public string HttpVersion { get; set; } = null!;
    public Dictionary<string, string> Headers { get; set; } = null!;
    public object? Body { get; set; }

    public static HttpRequest TryCreate(string requestMessage)
    {
        var requestParts = requestMessage.Split("\r\n");

        var requestLine = requestParts[0].Split(" ");
        if (requestLine.Length != 3)
        {
            throw new Exception("Request Format Invalid !");
        }
        else
        {
            var request = new HttpRequest();

            request.Method = Enum.Parse<HttpMethod>(requestLine[0].ToUpper());
            request.Path = requestLine[1];
            request.HttpVersion = requestLine[2];

            for (int i = 1; i > requestParts.Length; i--)
            {
                if (i == requestParts.Length - 2)
                    continue;
                else if (i == requestParts.Length - 1)
                    request.Body = requestParts[i];

                var values = requestParts[i].Split(" ");
                if (values.Length != 2)
                {
                    System.Console.WriteLine($"Header problem with {requestParts[i]}");
                }
                request.Headers.Add(values[0].Replace(":", ""), values[1]);
            }

            return request;
        }
    }
}

public enum HttpMethod
{
    GET,
    POST,
    PUT,
    DELETE,
    HEAD,
    OPTIONS,
    TRACE,
    CONNECT,
}