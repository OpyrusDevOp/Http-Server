namespace  Http_Server;
public class Router
{
    string Path;

    public Router(string path = "/")
    {
        // Ensure the base path does not have a trailing slash (except for the root "/")
        Path = path.EndsWith("/") && path.Length > 1 ? path.TrimEnd('/').Replace(" ", "") : path;
        if(!Path.StartsWith("/")) Path = "/" + Path;
    }

    Dictionary<string, Func<HttpRequest, string>> EndPoints = new();

    public Dictionary<string, Func<HttpRequest, string>> endPoints => EndPoints;

    public void AddEndpoint(HttpMethods methods, Func<HttpRequest, string> handler, string path = "/")
    {
        // Ensure the path starts with a single slash
        if (!path.StartsWith("/"))
        {
            path = "/" + path;
        }

        // Combine and sanitize the route
        var combinedPath = $"{Path}{path}".Replace("//", "/").TrimEnd('/');
        var routeKey = $"{methods}:{combinedPath}";

        if (EndPoints.ContainsKey(routeKey))
            throw new Exception($"This route already exists for method {methods}!");

        EndPoints.Add(routeKey, handler);
    }

    public void AddSubRoute(Router router)
    {
        foreach (var endpoint in router.EndPoints)
        {
            var originalKey = endpoint.Key;

            // Extract method and endpoint path
            var methodSeparatorIndex = originalKey.IndexOf(':');
            var method = originalKey.Substring(0, methodSeparatorIndex);
            var subPath = originalKey.Substring(methodSeparatorIndex + 1);

            // Combine paths and sanitize
            var combinedPath = $"{Path}{subPath}".Replace("//", "/");
            var routeKey = $"{method}:{combinedPath}";

            if (EndPoints.ContainsKey(routeKey))
                throw new Exception($"This route already exists for method {method}!");

            EndPoints.Add(routeKey, endpoint.Value);
        }
    }
}
