using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Http_Server;

public class HttpServer
{
    Dictionary<string, Func<HttpRequest, string>> Routes;

    TcpListener TcpServer = null!;
    int Port;

    int ThreadAmount = 4;

    //Queue<Socket> Requests;

    public HttpServer(int port)
    {
        Port = port;
        Routes = new Dictionary<string, Func<HttpRequest, string>>
        {
            {
                "GET:/css/{css}",
                req =>
                {
                    if (req.Params == null || !req.Params.TryGetValue("css", out var css))
                        return HttpResponses.NotFound();
                    return HttpResponses.View($"css/{css}");
                }
            },
        };
    }

    ///<summary>
    /// Launch the server
    ///</summary>
    public void Start()
    {
        ThreadPool.SetMaxThreads(ThreadAmount, ThreadAmount);
        TcpServer = new TcpListener(IPAddress.Any, Port);
        TcpServer.Start();
        Run();
    }

    void Run()
    {
        Console.WriteLine($"Server started at port : {Port}");
        while (true)
        {
            var socket = TcpServer.AcceptSocket();
            ThreadPool.QueueUserWorkItem(_ => TreatRequest(socket));
            //ThreadsRegulation();
        }
    }

    void TreatRequest(Socket socket)
    {
        var requestByte = new byte[1024];
        socket.Receive(requestByte);
        var requestString = Encoding.UTF8.GetString(requestByte);

        //var msg = new byte[0];

        var msg = HttpResponses.InternalError();

        try
        {
            var request = HttpRequest.TryCreate(requestString);
            Console.WriteLine(requestString);

            var handler = RouteHandler(
                request.Methods,
                request.Path,
                out Dictionary<string, object> parameters
            );

            request.Params = parameters;

            msg = handler == null ? HttpResponses.NotFound() : handler(request);
            var response = Encoding.UTF8.GetBytes(msg);
            socket.Send(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();

    }

    Func<HttpRequest, string>? RouteHandler(
        HttpMethods method,
        string path,
        out Dictionary<string, object> parameters
    )
    {
        path = path.TrimEnd('/');

        Dictionary<string, object> requestParams = new();
        var handler = Routes.FirstOrDefault(r =>
        {
            
            var requestSegments = path.Split("/");
            var endpointSegments = r.Key.Split(":");

            if (endpointSegments[0] != method.ToString()) return false;
            
            var routeSegments = endpointSegments[1].Split("/");

            if (routeSegments.Length != requestSegments.Length)
                return false;
            for (var i = 0; i < routeSegments.Length; i++)
            {
                if (routeSegments[i].StartsWith("{") && routeSegments[i].EndsWith("}"))
                {
                    var paramName = routeSegments[i][1..^1];
                    var paramValue = requestSegments[i];
                    //System.Console.WriteLine($"{paramName} = {paramValue}");
                    requestParams.Add(paramName, paramValue);
                }
                else if (i == routeSegments.Length - 1)
                {
                    var segmentParts = requestSegments[i].Split("?");

                    if (segmentParts[0] != routeSegments[i])
                        return false;
                    if (segmentParts.Length < 2)
                        continue;

                    var queryParameters = segmentParts[1].Split("&");

                    foreach (var query in queryParameters)
                    {
                        var keyValue = query.Split("=");

                        if (keyValue.Length < 2)
                            continue;

                        requestParams.Add(keyValue[0], keyValue[1]);
                    }
                }
                else if (routeSegments[i] != requestSegments[i])
                {
                    requestParams = new();
                    
                    return false;
                }
            }

            return true;
        });
        parameters = requestParams;

        return handler.Value;
    }

    /// <summary>
    /// For adding route to the HttpServer
    /// </summary>
    /// <param name="methods">The http method GET, POST etc.</param>
    /// <param name="path">The endpoint path e.g : "/api/users/getuser"</param>
    /// <param name="handler">The endpoint action handler</param>
    public void AddEnpoints(HttpMethods methods, string path, Func<HttpRequest, string> handler)
    {
        path = path.TrimEnd('/').Replace(" ", "");
        var routeKey = $"{methods}:{path}";

        if (Routes.ContainsKey(routeKey))
            throw new Exception($"This route already exist for method {methods} !");

        Routes.Add(routeKey, handler);
    }

    public void AddRouter(Router router)
    {
        foreach (var endpoint in router.endPoints)
        {
            var routeKey = endpoint.Key;

            if (Routes.ContainsKey(routeKey))
            {
                Console.WriteLine($"This route already exists for method this method!");
                continue;
            }

            Routes.Add(routeKey, endpoint.Value);
        }
    }

    /// <summary>
    /// Set number of thread to handle request
    /// </summary>
    /// <param name="amount">Max amount</param>
    public void SetMaxThreadAmount(int amount) => ThreadAmount = amount;

    //private void ThreadsRegulation()
    //{
    //  Threads.RemoveAll(t => !t.IsAlive);

    //var freeThreads = ThreadAmount - Threads.Count;

    //for (int i = 0; i < freeThreads && i < Requests.Count; i++)
    //{
    // var thread = new Thread(TreatRequest(Requests.Dequeue()));
    //  Threads.Add(thread);
    //    thread.Start();
    //  }
    //}
}
