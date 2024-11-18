public static class HttpResponses
{
    public static string View(string pageName)
    {
        var pagePath = Path.Combine(AppContext.BaseDirectory, "webroot", pageName);

        if (!File.Exists(pagePath))
            return NotFound();

        var pageContent = File.ReadAllText(pagePath);
        var contentType = GetContentType(pagePath);

        return Ok(contentType, pageContent);
    }

    static string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".html" => "text/html",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            _ => "application/octet-stream",
        };
    }

    ///<summary></summary>
    /// <returns> Status code 200 </returns>
    public static string Ok(string body = "") =>
        $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n{body}";

    public static string Ok(string contentType = "text/plain", string body = "") =>
        $"HTTP/1.1 200 OK\r\nContent-Type: {contentType}\r\n\r\n{body}";

    public static string Ok(Dictionary<string, string> headers, string body = "")
    {
        if (headers.Count < 1)
            return InternalError();

        var headerLine = "";

        foreach (var header in headers)
        {
            headerLine += $"{header.Key}: {header.Value}\r\n";
        }

        return $"HTTP/1.1 200 OK\r\n{headerLine}\r\n{body}";
    }

    ///<summary></summary>
    /// <returns> Status code 404 </returns>
    public static string NotFound(string message = "") =>
        $"HTTP/1.1 404 NotFound\r\nContent-Type: text/plain\r\nContent-Length: 3\r\n\r\n{message}";

    ///<summary></summary>
    /// <returns> Status code 400 </returns>
    public static string BadRequest(string message = "") =>
        $"HTTP/1.1 400 BadRequest\r\nContent-Type: text/plain\r\nContent-Length: 3\r\n\r\n{message}";

    ///<summary></summary>
    /// <returns> Status code 500 </returns>
    public static string InternalError(string message = "") =>
        $"HTTP/1.1 500 InternalServerError\r\nContent-Type: text/plain\r\nContent-Length: 3\r\n\r\n{message}";
}
