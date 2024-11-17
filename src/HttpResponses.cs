public static class HttpResponses
{
    ///<summary></summary>
    /// <returns> Status code 200 </returns>
    public static string Ok(string body = "") =>
        $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n{body}";

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
