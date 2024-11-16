using System.Net;
using System.Net.Sockets;
using System.Text;

public class HttpServer
{
    private TcpListener TcpServer = null!;
    private int Port;

    int ThreadAmount = 4;

    //Queue<Socket> Requests;

    public HttpServer(int port)
    {
        Port = port;
        //Requests = new();
        ThreadPool.SetMaxThreads(ThreadAmount, ThreadAmount);
    }

    public void Start()
    {
        TcpServer = new TcpListener(IPAddress.Any, Port);
        TcpServer.Start();
        Run();
    }

    void Run()
    {
        while (true)
        {
            var socket = TcpServer.AcceptSocket();
            ThreadPool.QueueUserWorkItem(_ => TreatRequest(socket));

            //ThreadsRegulation();
        }
    }

    private void TreatRequest(Socket socket)
    {
        var requestByte = new byte[1024];
        socket.Receive(requestByte);
        var requestString = Encoding.UTF8.GetString(requestByte);
        var requestParts = requestString.Split("\r\n");

        //var msg = new byte[0];

        var msg = "HTTP/1.1 200 OK\r\n\r\n";

        try
        {
            var request = HttpRequest.TryCreate(requestString);
            System.Console.WriteLine(requestString);
        }
        catch (System.Exception e)
        {
            System.Console.WriteLine(e.Message);
            msg = "HTTP/1.1 400 BAD REQUEST\r\n\r\n";
        }

        var response = Encoding.UTF8.GetBytes(msg);
        socket.Send(response, 0, response.Length, SocketFlags.None);
        socket.Close();
    }

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
