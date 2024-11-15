// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using System.Text;

var tcpServer = new TcpListener(IPAddress.Any, 3000);

tcpServer.Start();

while (true)
{
    var socket = tcpServer.AcceptSocket();
    var requestByte = new byte[1024];
    socket.Receive(requestByte);
    var request = System.Text.Encoding.UTF8.GetString(requestByte);
    var requestParts = request.Split("\r\n");

    var requestLine = requestParts[0].Split(" ");

    var msg = new byte[0];
    if (requestLine.Length != 3)
    {
        Console.WriteLine("Request Format Request !");
        msg = Encoding.UTF8.GetBytes("Only HTTP request accepted");
    }
    else
    {
        Console.WriteLine(request);
        var path = requestLine[1];
        if (path.Length > 1 && path != "/index")
            msg = Encoding.UTF8.GetBytes("HTTP/1.1 404 NOT FOUND\r\n\r\n");
        else
            msg = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n\r\n");
    }

    //var msg = System.Text.Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n\r\n");
    socket.Send(msg, 0, msg.Length, SocketFlags.None);
    socket.Close();
}
