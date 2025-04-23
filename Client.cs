using System.Net.Sockets;

namespace KursuchServer;

public struct Client
{
    public String Login;
    public String Password;
    public String AdminKey;
    public TcpClient Cocket;
}