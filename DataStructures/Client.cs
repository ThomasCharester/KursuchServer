using System.Net.Sockets;

namespace KursuchServer;

public struct Client
{
    public String Login;
    public String Password;
    public String AdminKey;
    public TcpClient Cocket;
    
    public Client(){}

    public Client(String login, String password, String adminKey, TcpClient cocket = null)
    {
        Login = login;
        Password = password;
        AdminKey = adminKey;
        Cocket = cocket;
    }
}