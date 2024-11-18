using System.Net;
using System.Net.Sockets;
using System.Text;

public class HttpServer
{
    readonly Dictionary<string, Func<HttpRequest, string>> Routes;

    TcpListener TcpServer = null!;
    int Port;

    int ThreadAmount = 4;

    //Queue<Socket> Requests;

    public HttpServer(int port)
    {
        Port = port;
        Routes = new();
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
        var requestParts = requestString.Split("\r\n");

        //var msg = new byte[0];

        var msg = HttpResponses.InternalError();

        try
        {
            var request = HttpRequest.TryCreate(requestString);
            Console.WriteLine(requestString);

            var handler = RouteHandler(request.Method, request.Path);

            msg = handler == null ? HttpResponses.NotFound() : handler(request);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            msg = HttpResponses.BadRequest(e.Message);
        }

        var response = Encoding.UTF8.GetBytes(msg);
        socket.Send(response, 0, response.Length, SocketFlags.None);
        socket.Close();
    }

    Func<HttpRequest, string>? RouteHandler(HttpMethod method, string path)
    {
        path = path.TrimEnd('/');
        var routeKey = $"{method}:{path}";

        return Routes.GetValueOrDefault(routeKey);
    }

    /// <summary>
    /// For adding route to the HttpServer
    /// </summary>
    /// <param name="method">The http method GET, POST etc.</param>
    /// <param name="path">The endpoint path e.g : "/api/users/getuser"</param>
    /// <param name="handler">The endpoint action handler</param>
    public void AddEnpoints(HttpMethod method, string path, Func<HttpRequest, string> handler)
    {
        path = path.TrimEnd('/').Replace(" ", "");
        var routeKey = $"{method}:{path}";

        if (Routes.ContainsKey(routeKey))
            throw new Exception($"This route already exist for method {method} !");

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
