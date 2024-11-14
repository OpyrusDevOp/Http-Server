// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;

var tcpServer = new TcpListener(IPAddress.Any, 3000);

tcpServer.Start();

while (true)
{
    var socket = tcpServer.AcceptSocket();
    var msg = System.Text.Encoding.UTF8.GetBytes("Connection Succeed !!");
    socket.Send(msg, 0, msg.Length, SocketFlags.None);
    socket.Close();
}
